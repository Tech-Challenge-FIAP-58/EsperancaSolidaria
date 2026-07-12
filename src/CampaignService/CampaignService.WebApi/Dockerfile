# =============================================================
# ARGs globais — disponíveis em todos os stages
# =============================================================
ARG DOTNET_VERSION=10.0
ARG BUILD_CONFIGURATION=Development
ARG APP_VERSION=1.0.0

# IMPORTANTE: o contexto de build é a pasta `src/` (monorepo), não `src/CampaignService`,
# porque o serviço referencia o projeto compartilhado Shared/EsperancaSolidaria.Contracts.
# No CI: `docker build -f CampaignService.WebApi/Dockerfile ..` (contexto = src/).

# =============================================================
# STAGE 1: Base runtime
# =============================================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base

# Usuário não-root — compatível com Alpine e Debian (imagem aspnet usa Debian)
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

WORKDIR /app

RUN chown -R appuser:appgroup /app

EXPOSE 8080

# =============================================================
# STAGE 2: Restore de dependências (layer altamente cacheável)
# Só re-executa se algum .csproj mudar
# =============================================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore

WORKDIR /src

# .csproj copiados preservando a estrutura do monorepo (serviço + contrato compartilhado)
COPY CampaignService/CampaignService.WebApi/CampaignService.WebApi.csproj CampaignService/CampaignService.WebApi/
COPY CampaignService/CampaignService.Application/CampaignService.Application.csproj CampaignService/CampaignService.Application/
COPY CampaignService/CamapaignService.Infra/CamapaignService.Infra.csproj CampaignService/CamapaignService.Infra/
COPY CampaignService/CampaignService.Domain/CampaignService.Domain.csproj CampaignService/CampaignService.Domain/
COPY Shared/EsperancaSolidaria.Contracts/EsperancaSolidaria.Contracts.csproj Shared/EsperancaSolidaria.Contracts/

RUN dotnet restore CampaignService/CampaignService.WebApi/CampaignService.WebApi.csproj

# =============================================================
# STAGE 3: Build + Publish
# =============================================================
FROM restore AS publish

ARG BUILD_CONFIGURATION

# Código-fonte dos projetos usados: o serviço e os contratos compartilhados
COPY CampaignService/ CampaignService/
COPY Shared/ Shared/

RUN dotnet publish CampaignService/CampaignService.WebApi/CampaignService.WebApi.csproj \
    -c ${BUILD_CONFIGURATION} \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# =============================================================
# STAGE 4: Runtime final (imagem mínima)
# =============================================================
FROM base AS final

# Labels OCI — rastreabilidade no DockerHub, Kubernetes e pipelines CI/CD
ARG APP_VERSION
LABEL org.opencontainers.image.title="CampaignService.WebApi" \
      org.opencontainers.image.description="Microsserviço de campanhas - Esperança Solidária" \
      org.opencontainers.image.version="${APP_VERSION}" \
      org.opencontainers.image.vendor="Thiago Olszewski" \
      org.opencontainers.image.source="https://github.com/Tech-Challenge-FIAP-58/EsperancaSolidaria"

WORKDIR /app

# Copia artefatos publicados com ownership já correto
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Variáveis de ambiente de runtime do .NET
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false

USER appuser

# Health check via wget (disponível na imagem aspnet — curl NÃO está)
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CampaignService.WebApi.dll"]
