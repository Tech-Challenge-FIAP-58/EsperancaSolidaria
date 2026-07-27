# Provisiona no Zabbix os hosts e itens dos 3 microsservicos.
#
# Cada host tem um item mestre (HTTP agent) que baixa o /metrics da API, e itens
# dependentes que extraem cada metrica via pre-processamento "Prometheus pattern".
# Assim o Zabbix coleta sem precisar de agent dentro dos pods.
#
# Uso: .\k8s\infra\zabbix-provision.ps1

param(
    [string]$ZabbixUser = 'Admin',
    [string]$ZabbixPassword = 'zabbix',
    [int]$LocalPort = 8080
)

$ErrorActionPreference = 'Stop'
$api = "http://localhost:$LocalPort/api_jsonrpc.php"
$script:token = $null

function Invoke-Zbx {
    param([string]$Method, $Params = @{})

    $headers = @{ 'Content-Type' = 'application/json-rpc' }
    if ($script:token) { $headers['Authorization'] = "Bearer $script:token" }

    $body = @{ jsonrpc = '2.0'; method = $Method; params = $Params; id = 1 } | ConvertTo-Json -Depth 12
    $response = Invoke-RestMethod -Uri $api -Method Post -Headers $headers -Body $body -TimeoutSec 30

    if ($response.PSObject.Properties.Name -contains 'error') {
        throw "$Method falhou: $($response.error.message) $($response.error.data)"
    }
    return $response.result
}

$services = @(
    @{ Host = 'campaign-service'; Display = 'Campaign Service'; Dns = 'campaign-service.services.svc.cluster.local' }
    @{ Host = 'donation-service'; Display = 'Donation Service'; Dns = 'donation-service.services.svc.cluster.local' }
    @{ Host = 'user-service';     Display = 'User Service';     Dns = 'user-service.services.svc.cluster.local' }
)

# Mode 'value' pega a unica serie que casa com o pattern.
# Mode 'function' + Func agrega as varias series (http_requests_received_total tem
# uma serie por combinacao de code/method/endpoint).
$metrics = @(
    @{ Key = 'cpu.seconds';   Name = 'Process CPU Seconds'; Pattern = 'process_cpu_seconds_total';    Mode = 'value';    Units = 's' }
    @{ Key = 'threads';       Name = 'Threads Count';       Pattern = 'process_num_threads';          Mode = 'value';    Units = '' }
    @{ Key = 'http.requests'; Name = 'HTTP Total Requests'; Pattern = 'http_requests_received_total'; Mode = 'function'; Func = 'sum'; Units = '' }
    @{ Key = 'memory';        Name = 'Memory Usage';        Pattern = 'process_working_set_bytes';    Mode = 'value';    Units = 'B' }
)

$groupName = 'Esperanca Solidaria'

Write-Host "`n== Abrindo port-forward para o Zabbix ==" -ForegroundColor Cyan
$pf = Start-Job { param($p) kubectl port-forward -n monitoring svc/zabbix-zabbix-web "${p}:80" } -ArgumentList $LocalPort

try {
    # zabbix-web nao tem readiness probe: espera a API responder de fato
    $deadline = (Get-Date).AddSeconds(180)
    while ($true) {
        Start-Sleep -Seconds 5
        try { Invoke-Zbx 'apiinfo.version' | Out-Null; break }
        catch {
            if ((Get-Date) -ge $deadline) { throw "Zabbix nao respondeu em 180s: $_" }
            Write-Host "    aguardando a API do Zabbix..."
        }
    }

    Write-Host "`n== Autenticando ==" -ForegroundColor Cyan
    $script:token = Invoke-Zbx 'user.login' @{ username = $ZabbixUser; password = $ZabbixPassword }
    Write-Host "    autenticado como $ZabbixUser"

    Write-Host "`n== Host group ==" -ForegroundColor Cyan
    $group = Invoke-Zbx 'hostgroup.get' @{ filter = @{ name = @($groupName) } }
    if ($group) {
        $groupId = $group[0].groupid
        Write-Host "    '$groupName' ja existe (id $groupId)"
    }
    else {
        $groupId = (Invoke-Zbx 'hostgroup.create' @{ name = $groupName }).groupids[0]
        Write-Host "    '$groupName' criado (id $groupId)"
    }

    foreach ($svc in $services) {
        Write-Host "`n== $($svc.Display) ==" -ForegroundColor Cyan

        # recria do zero para o script ser idempotente
        $existing = Invoke-Zbx 'host.get' @{ filter = @{ host = @($svc.Host) } }
        if ($existing) {
            Invoke-Zbx 'host.delete' @($existing[0].hostid) | Out-Null
            Write-Host "    host anterior removido"
        }

        $hostId = (Invoke-Zbx 'host.create' @{
                host   = $svc.Host
                name   = $svc.Display
                groups = @(@{ groupid = $groupId })
            }).hostids[0]
        Write-Host "    host criado (id $hostId)"

        # item mestre: baixa o /metrics inteiro uma vez por minuto
        $masterId = (Invoke-Zbx 'item.create' @{
                hostid     = $hostId
                name       = 'Prometheus metrics (raw)'
                key_       = 'prometheus.metrics'
                type       = 19            # HTTP agent
                value_type = 4             # text
                url        = "http://$($svc.Dns)/metrics"
                delay      = '1m'
                history    = '1d'
            }).itemids[0]
        Write-Host "    item mestre criado (id $masterId)"

        foreach ($m in $metrics) {
            $params = if ($m.Mode -eq 'function') { "$($m.Pattern)`nfunction`n$($m.Func)" }
                      else { "$($m.Pattern)`nvalue" }

            Invoke-Zbx 'item.create' @{
                hostid        = $hostId
                name          = $m.Name
                key_          = "esperanca.$($m.Key)"
                type          = 18         # dependent
                value_type    = 0          # numeric float
                master_itemid = $masterId
                units         = $m.Units
                history       = '7d'
                trends        = '30d'
                preprocessing = @(
                    @{
                        type               = 22    # Prometheus pattern
                        params             = $params
                        error_handler      = 0
                        error_handler_params = ''
                    }
                )
            } | Out-Null
            Write-Host "    item '$($m.Name)' <- $($m.Pattern)"
        }
    }

    Write-Host "`nProvisionamento concluido. Os primeiros valores chegam em ate 1 minuto." -ForegroundColor Green
    Write-Host "Zabbix : http://localhost:30808  (Admin / zabbix)"
    Write-Host "Grafana: http://localhost:30300/d/esperanca-solidaria  (admin / esperancagrafana123)"
}
finally {
    Stop-Job $pf -ErrorAction SilentlyContinue
    Remove-Job $pf -ErrorAction SilentlyContinue
}
