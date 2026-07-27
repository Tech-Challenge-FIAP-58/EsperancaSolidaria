# Sobe todo o ambiente Kubernetes do Esperanca Solidaria
# Uso: .\k8s\deploy.ps1

$ErrorActionPreference = 'Stop'
$k8s = $PSScriptRoot

Write-Host "`n== Namespaces ==" -ForegroundColor Cyan
kubectl apply -f "$k8s\namespaces.yaml"

Write-Host "`n== Infraestrutura (RabbitMQ, Grafana, Zabbix) ==" -ForegroundColor Cyan
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

Write-Host "`n== Status ==" -ForegroundColor Cyan
kubectl get pods,svc -n services
kubectl get pods,svc -n monitoring

Write-Host "`nCampaign : http://localhost:30080/swagger"
Write-Host "Donation : http://localhost:30081/swagger"
Write-Host "User     : http://localhost:30082/swagger"
