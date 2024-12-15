using lab6.Data;
using Microsoft.EntityFrameworkCore;
namespace lab6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // �������� ������ �����������
            var connectionString = builder.Configuration.GetConnectionString("DBConnection")
                ?? throw new InvalidOperationException("Connection string 'DBConnection' not found.");

            // ��������� DbContext ��� ������ � ����� ������
            builder.Services.AddDbContext<ProdajnikContext>(options =>
                options.UseSqlServer(connectionString));

            // ��������� ������� ��� ������ � ������������� � ���������������
            builder.Services.AddControllersWithViews(); // ��� ������ � ������������� � ���������������

            // ��������� Swagger � ���������� ���������
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations(); // �������� ��������� Swagger
            });

            var app = builder.Build();

            // ������������ HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // ��������� Swagger JSON
                app.UseSwaggerUI(); // ��������� ��� �������������� � API ����� Swagger
            }

            app.UseAuthorization();

            // �������� ��� ������������
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=ConstructionObj}/{action=index}/{id?}"); // ���������, ��� �� ��������� ����� ���������� ����� Index � ����������� CunstructionObject

            app.Run();
        }
    }
}
