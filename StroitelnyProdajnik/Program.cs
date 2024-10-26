using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using StroitelnyProdajnik.Data;
using StroitelnyProdajnik.Service;
using Microsoft.AspNetCore.Http;

namespace StroitelnyProdajnik
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ������ ������ ����������� �� appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DBConnection");

            // ����������� ��������
            builder.Services.AddDbContext<BuilderPodContext>(options =>
                options.UseSqlServer(connectionString));

            // ����������� ����������� � ������
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CachedDataService>();

            // ����������� ������
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>������� ��������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY>";
                    strResponse += "<BR><A href='/table'>�������</A>";
                    strResponse += "<BR><A href='/info'>����������</A>";
                    strResponse += "<BR><A href='/searchform1'>SearchForm1</A>";
                    strResponse += "<BR><A href='/searchform2'>SearchForm2</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            // ��������� ���� "/searchform1"
            app.Map("/searchform1", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<BuilderPodContext>();

                    if (context.Request.Method == "GET")
                    {
                        var objectName = context.Request.Cookies["ObjectName"] ?? "";
                        var selectedCustomerId = context.Request.Cookies["CustomerId"] ?? "";
                        var selectedWorkLists = context.Request.Cookies["WorkLists"]?.Split(',') ?? Array.Empty<string>();

                        var customers = await dbContext.Customers.ToListAsync();
                        var workLists = await dbContext.ConstructionObjects.Select(co => co.WorkList).Distinct().ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 1</title></head><body>";
                        html += "<h1>����� �������� �������������</h1>";
                        html += "<form method='post'>";
                        html += "<label for='ObjectName'>������������ �������:</label><br/>";
                        html += $"<input type='text' id='ObjectName' name='ObjectName' value='{objectName}' /><br/><br/>";

                        html += "<label for='CustomerId'>��������:</label><br/>";
                        html += "<select id='CustomerId' name='CustomerId'>";
                        foreach (var customer in customers)
                        {
                            var selected = customer.CustomerId.ToString() == selectedCustomerId ? "selected" : "";
                            html += $"<option value='{customer.CustomerId}' {selected}>{customer.OrganizationName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='WorkLists'>������ �����:</label><br/>";
                        html += "<select id='WorkLists' name='WorkLists' multiple size='5'>";
                        foreach (var work in workLists)
                        {
                            var selected = selectedWorkLists.Contains(work) ? "selected" : "";
                            html += $"<option value='{work}' {selected}>{work}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>�����</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var objectName = formData["ObjectName"];
                        var customerId = formData["CustomerId"];
                        var workLists = formData["WorkLists"];

                        context.Response.Cookies.Append("ObjectName", objectName);
                        context.Response.Cookies.Append("CustomerId", customerId);
                        context.Response.Cookies.Append("WorkLists", string.Join(",", workLists));

                        var query = dbContext.ConstructionObjects.Include(co => co.Customer).AsQueryable();

                        if (!string.IsNullOrEmpty(objectName))
                        {
                            query = query.Where(co => co.ObjectName.Contains(objectName));
                        }

                        if (int.TryParse(customerId, out int customerIdValue))
                        {
                            query = query.Where(co => co.CustomerId == customerIdValue);
                        }

                        if (workLists.Count > 0)
                        {
                            query = query.Where(co => workLists.Any(w => co.WorkList.Contains(w)));
                        }

                        var results = await query.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>���������� ������</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>������������ �������</th><th>��������</th><th>������ �����</th></tr>";
                            foreach (var constructionObject in results)
                            {
                                html += "<tr>";
                                html += $"<td>{constructionObject.ObjectId}</td>";
                                html += $"<td>{constructionObject.ObjectName}</td>";
                                html += $"<td>{constructionObject.Customer?.OrganizationName}</td>";
                                html += $"<td>{constructionObject.WorkList}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>���������� �� �������.</p>";
                        }

                        html += "<br/><a href='/searchform1'>����� � ������</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });




            // ��������� ���� "/searchform2" � �������������� ������
            app.Map("/searchform2", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<BuilderPodContext>();

                    if (context.Request.Method == "GET")
                    {
                        var objectName = context.Session.GetString("ObjectName") ?? "";
                        var selectedCustomerId = context.Session.GetString("CustomerId") ?? "";
                        var selectedWorkLists = context.Session.GetString("WorkLists")?.Split(',') ?? Array.Empty<string>();

                        var customers = await dbContext.Customers.ToListAsync();
                        var workLists = await dbContext.ConstructionObjects.Select(co => co.WorkList).Distinct().ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 2</title></head><body>";
                        html += "<h1>����� �������� ������������� (Session)</h1>";
                        html += "<form method='post'>";
                        html += "<label for='ObjectName'>������������ �������:</label><br/>";
                        html += $"<input type='text' id='ObjectName' name='ObjectName' value='{objectName}' /><br/><br/>";

                        html += "<label for='CustomerId'>��������:</label><br/>";
                        html += "<select id='CustomerId' name='CustomerId'>";
                        foreach (var customer in customers)
                        {
                            var selected = customer.CustomerId.ToString() == selectedCustomerId ? "selected" : "";
                            html += $"<option value='{customer.CustomerId}' {selected}>{customer.OrganizationName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='WorkLists'>������ �����:</label><br/>";
                        html += "<select id='WorkLists' name='WorkLists' multiple size='5'>";
                        foreach (var work in workLists)
                        {
                            var selected = selectedWorkLists.Contains(work) ? "selected" : "";
                            html += $"<option value='{work}' {selected}>{work}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>�����</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var objectName = formData["ObjectName"];
                        var customerId = formData["CustomerId"];
                        var workLists = formData["WorkLists"];

                        context.Session.SetString("ObjectName", objectName);
                        context.Session.SetString("CustomerId", customerId);
                        context.Session.SetString("WorkLists", string.Join(",", workLists));

                        var query = dbContext.ConstructionObjects.Include(co => co.Customer).AsQueryable();

                        if (!string.IsNullOrEmpty(objectName))
                        {
                            query = query.Where(co => co.ObjectName.Contains(objectName));
                        }

                        if (int.TryParse(customerId, out int customerIdValue))
                        {
                            query = query.Where(co => co.CustomerId == customerIdValue);
                        }

                        if (workLists.Count > 0)
                        {
                            query = query.Where(co => workLists.Any(w => co.WorkList.Contains(w)));
                        }

                        var results = await query.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>���������� ������</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>������������ �������</th><th>��������</th><th>������ �����</th></tr>";
                            foreach (var constructionObject in results)
                            {
                                html += "<tr>";
                                html += $"<td>{constructionObject.ObjectId}</td>";
                                html += $"<td>{constructionObject.ObjectName}</td>";
                                html += $"<td>{constructionObject.Customer?.OrganizationName}</td>";
                                html += $"<td>{constructionObject.WorkList}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>���������� �� �������.</p>";
                        }

                        html += "<br/><a href='/searchform2'>����� � ������</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });




            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/table")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>�������</TITLE></HEAD>" +
                     "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                     "<BODY>";
                    strResponse += "<BR><A href='/table/BuildingMaterials'>������������ ���������</A>";
                    strResponse += "<BR><A href='/table/ConstructionObjects'>������������ �������</A>";
                    strResponse += "<BR><A href='/table/Customers'>���������</A>";
                    strResponse += "<BR><A href='/table/ObjectMaterials'>��������� ��������</A>";
                    strResponse += "<BR><A href='/table/ObjectWorks'>������ ��������</A>";
                    strResponse += "<BR><A href='/table/WorkTypes'>���� �����</A>";
                    strResponse += "<BR><A href='/table/FullConstructionObjectInfo'>������ ���������� � ������������ ��������</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/table", out var remainingPath) && remainingPath.HasValue && remainingPath.Value.StartsWith("/"))
                {
                    context.Response.ContentType = "text/html; charset=utf-8"; // ��������� Content-Type
                    var tableName = remainingPath.Value.Substring(1); // ������� ��������� ����

                    var cachedService = context.RequestServices.GetService<CachedDataService>();

                    if (tableName == "BuildingMaterials")
                    {
                        var list = cachedService.GetBuildingMaterials();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "ConstructionObjects")
                    {
                        var list = cachedService.GetConstructionObjects();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "Customers")
                    {
                        var list = cachedService.GetCustomers();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "ObjectMaterials")
                    {
                        var list = cachedService.GetObjectMaterials();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "ObjectWorks")
                    {
                        var list = cachedService.GetObjectWorks();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "WorkTypes")
                    {
                        var list = cachedService.GetWorkTypes();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "FullConstructionObjectInfo")
                    {
                        var list = cachedService.GetFullConstructionObjectInfo();
                        await RenderTable(context, list);
                    }
                    else
                    {
                        // ���� ������� �� �������, ���������� 404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("������� �� �������");
                    }

                    return; // ��������� ��������� �������
                }
                await next.Invoke();
            });


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/info")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>����������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>����������:</H1>";
                    strResponse += "<BR> ������: " + context.Request.Host;
                    strResponse += "<BR> ����: " + context.Request.Path;
                    strResponse += "<BR> ��������: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            async Task RenderTable<T>(HttpContext context, IEnumerable<T> data)
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                var html = "<table border='1' style='border-collapse:collapse'>";

                var type = typeof(T);

                // ��������� ���������� ������� �� ������ ������� ����
                html += "<tr>";
                foreach (var prop in type.GetProperties())
                {
                    // ���������� ��������, ������� �������� ��������� ������ ������� ��� �����������
                    if (!IsSimpleType(prop.PropertyType))
                    {
                        continue;
                    }

                    html += $"<th>{prop.Name}</th>";
                }
                html += "</tr>";

                foreach (var item in data)
                {
                    html += "<tr>";
                    foreach (var prop in type.GetProperties())
                    {
                        if (!IsSimpleType(prop.PropertyType))
                        {
                            continue;
                        }

                        var value = prop.GetValue(item);

                        if (value is DateTime dateValue)
                        {
                            html += $"<td>{dateValue.ToString("dd.MM.yyyy")}</td>";
                        }
                        else
                        {
                            html += $"<td>{value}</td>";
                        }
                    }
                    html += "</tr>";
                }

                html += "</table>";
                await context.Response.WriteAsync(html);
            }

            bool IsSimpleType(Type type)
            {
                // ����������� ���� � ����, ������� ��������� �������� (string, DateTime � �.�.)
                return type.IsPrimitive ||
                       type.IsValueType ||
                       type == typeof(string) ||
                       type == typeof(DateTime) ||
                       type == typeof(decimal);
            }

            app.Run();
        }
    }
}