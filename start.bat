@echo off
echo Import images
docker load -i ./ai-sorter-backend.tar
docker load -i ./ai-sorter-frontend.tar
docker load -i ./adminer.tar
docker load -i ./postgres.tar
echo Running docker
docker compose up -d --build
echo Initialize LLM
docker exec ai-ollama ollama run gemma2