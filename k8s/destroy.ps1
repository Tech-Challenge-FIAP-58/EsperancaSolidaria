# Remove todo o ambiente Kubernetes do Esperanca Solidaria
# Uso: .\k8s\destroy.ps1

$ErrorActionPreference = 'Continue'
$k8s = $PSScriptRoot

Write-Host "`n== Microsservicos ==" -ForegroundColor Cyan
kubectl delete -f "$k8s\services\user-service\" --ignore-not-found
kubectl delete -f "$k8s\services\donation-service\" --ignore-not-found
kubectl delete -f "$k8s\services\campaign-service\" --ignore-not-found

Write-Host "`n== Config e secrets compartilhados ==" -ForegroundColor Cyan
kubectl delete -f "$k8s\services\secrets.yaml" --ignore-not-found
kubectl delete -f "$k8s\services\configmap.yaml" --ignore-not-found

Write-Host "`n== Infraestrutura ==" -ForegroundColor Cyan
kubectl delete -f "$k8s\infra\" --ignore-not-found

Write-Host "`n== Namespaces (apaga tambem os PVCs) ==" -ForegroundColor Cyan
kubectl delete -f "$k8s\namespaces.yaml" --ignore-not-found

Write-Host "`n== Status ==" -ForegroundColor Cyan
kubectl get ns services monitoring --ignore-not-found

Write-Host "`nAmbiente removido."
# O banco esta no Atlas. Se voltar a usar o Mongo local:
# Write-Host "Para parar: docker compose -f src\docker-compose.yml --profile local-mongo down"
