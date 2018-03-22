FROM microsoft/aspnetcore:2.0
#拷贝项目publish文件夹中的所有文件到 docker容器中的publish文件夹中 
COPY . /publish
 
#设置工作目录为 `/publish` 文件夹，即容器启动默认的文件夹
WORKDIR /publish
 
#设置Docker容器对外暴露5000端口
EXPOSE 5000

#设置环境变量
ENV ASPNETCORE_URLS http://*:5000

#修改时区
ENV TZ=Asia/Shanghai

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["sh", "start.sh"]