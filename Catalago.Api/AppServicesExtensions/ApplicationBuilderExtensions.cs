namespace Catalago.Api.AppServicesExtensions

{
    public static class ApplicationBuilderExtensions
    {
        //Metodos de Extenssão com serviços para tratamento de Exception
        public static IApplicationBuilder UserExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

        //Habilita acesso externo dor Cors com istancias de IApplicationBuilder
        public static IApplicationBuilder UserAppCors(this IApplicationBuilder app)
        {
            app.UseCors(p =>
            {
                p.AllowAnyOrigin();
                p.WithMethods("GET");
                p.AllowAnyMethod();
            });
            return app;
        }

        //Faz registro do Swagger
        public static IApplicationBuilder UseSwaggerEndpoints(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { });

            return app;
        }
    }
}