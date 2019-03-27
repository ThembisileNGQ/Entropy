FROM nginx:latest
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY build /usr/share/nginx/html

# in *.sln current working directory run these!
# docker build -t lutando/laundry-frontend:latest -f Dockerfile .
# docker push lutando/laundry-frontend:latest