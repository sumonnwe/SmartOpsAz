
FROM node:22-alpine AS build

WORKDIR /app

COPY src/WebUI/SmartOpsAz.WebUI/SmartOpsAz.WebUI.Client/package*.json ./

RUN npm ci

COPY src/WebUI/SmartOpsAz.WebUI/SmartOpsAz.WebUI.Client/ ./

RUN npm run build

FROM nginx:alpine AS runtime

COPY src/WebUI/SmartOpsAz.WebUI/SmartOpsAz.WebUI.Client/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
