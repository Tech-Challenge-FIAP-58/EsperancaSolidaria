# Sobe todo o ambiente Kubernetes do Esperanca Solidaria
# Uso: .\k8s\deploy.ps1

$ErrorActionPreference = 'Stop'
$k8s = $PSScriptRoot

Write-Host "`n== Namespaces ==" -ForegroundColor Cyan
kubectl apply -f "$k8s\namespaces.yaml"

# Job concluido nao roda de novo no apply: apaga antes para reimportar o template
kubectl delete job zabbix-import -n monitoring --ignore-not-found

Write-Host "`n== Infraestrutura (RabbitMQ, Prometheus, Zabbix, Grafana) ==" -ForegroundColor Cyan
kubectl apply -f "$k8s\infra\"

Write-Host "`n== Config e secrets compartilhados ==" -ForegroundColor Cyan
kubectl apply -f "$k8s\services\configmap.yaml"
kubectl apply -f "$k8s\services\secrets.yaml"

Write-Host "`n== Microsservicos ==" -ForegroundColor Cyan
kubectl apply -f "$k8s\services\campaign-service\"
kubectl apply -f "$k8s\services\donation-service\"
kubectl apply -f "$k8s\services\user-service\"

Write-Host "`n== Aguardando pods ==" -ForegroundColor Cyan
kubectl rollout status deployment/rabbitmq -n services
kubectl rollout status deployment/campaign-deployment -n services
kubectl rollout status deployment/donation-deployment -n services
kubectl rollout status deployment/user-deployment -n services

Write-Host "`n== Aguardando monitoramento ==" -ForegroundColor Cyan
kubectl rollout status deployment/prometheus -n monitoring
kubectl rollout status deployment/zabbix-db -n monitoring
kubectl rollout status deployment/zabbix-server -n monitoring
kubectl rollout status deployment/zabbix-web -n monitoring
kubectl rollout status deployment/grafana -n monitoring

Write-Host "`n== Importando configuracao do Zabbix ==" -ForegroundColor Cyan
kubectl wait --for=condition=complete job/zabbix-import -n monitoring --timeout=900s
kubectl logs job/zabbix-import -n monitoring

Write-Host "`n== Status ==" -ForegroundColor Cyan
kubectl get pods,svc -n services
kubectl get pods,svc -n monitoring

Write-Host "`nCampaign : http://localhost:30080/swagger"
Write-Host "Donation : http://localhost:30081/swagger"
Write-Host "User     : http://localhost:30082/swagger"
Write-Host ""
Write-Host "Dashboard  : http://localhost:30300/d/esperanca-solidaria  (admin / esperancagrafana123)"
Write-Host "Prometheus : http://localhost:30900"
Write-Host "Zabbix     : http://localhost:30808  (Admin / zabbix)"
