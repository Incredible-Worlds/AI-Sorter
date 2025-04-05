@echo off
echo Running ollama
taskkill /IM ollama.exe /F
start cmd /k ollama serve
echo Import images
docker load -i ./ai-sorter-backend.tar
docker load -i ./ai-sorter-frontend.tar
docker load -i ./adminer.tar
docker load -i ./postgres.tar
echo Running docker
docker compose up -d --build