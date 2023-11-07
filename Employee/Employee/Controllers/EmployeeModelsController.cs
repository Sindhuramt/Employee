using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Employee.Models;

namespace Employee.Controllers
{
    public class EmployeeModelsController : Controller
    {
        private readonly string connectionString;

        public EmployeeModelsController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("EmployeeContext");
        }

        public async Task<IActionResult> Index()
        {
            var employees = await GetEmployeesFromDatabaseAsync();
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeModel employeeModel, IFormFile Picture)
        {
            if (Picture != null && Picture.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(Picture.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Picture.CopyToAsync(stream);
                }

                employeeModel.Picture = fileName;
            }

            DateTime? dateOfBirth = employeeModel.DateOfBirth;
            InsertEmployeeToDatabase(employeeModel.Name, dateOfBirth, employeeModel.Email, employeeModel.Picture , employeeModel.DateOfJoining);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var employee = await GetEmployeeFromDatabaseAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeModel employeeModel, IFormFile Picture)
        {
            if (Picture != null && Picture.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(Picture.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Picture.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(employeeModel.Picture))
                {
                    // Delete the old file if it exists
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", employeeModel.Picture);
                    System.IO.File.Delete(oldFilePath);
                }

                employeeModel.Picture = fileName;
            }

            DateTime? dateOfBirth = employeeModel.DateOfBirth;
            UpdateEmployeeInDatabase(employeeModel.ID, employeeModel.Name, dateOfBirth, employeeModel.Email, employeeModel.Picture , employeeModel.DateOfJoining);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var employee = await GetEmployeeFromDatabaseAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await GetEmployeeFromDatabaseAsync(id);
            if (employee != null && !string.IsNullOrEmpty(employee.Picture))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", employee.Picture);
                System.IO.File.Delete(filePath);
            }

            if (DeleteEmployeeFromDatabase(id))
            {
                return RedirectToAction(nameof(Index));
            }

            return Problem("Entity set 'EmployeeContext.EmployeeModel' is null.");
        }

        private async Task<EmployeeModel> GetEmployeeFromDatabaseAsync(int id)
        {
            var employees = new EmployeeModel();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetEmployeeById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.ID = (int)reader["ID"];
                            employees.Name = reader["Name"].ToString();
                            employees.DateOfBirth = reader["DateOfBirth"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["DateOfBirth"]);
                            employees.DateOfJoining = reader["DateOfJoining"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["DateOfJoining"]);
                            employees.Email = reader["Email"].ToString();
                            employees.Picture = reader["Picture"].ToString();
                        }
                    }
                }
            }

            return employees;
        }

        private async Task<List<EmployeeModel>> GetEmployeesFromDatabaseAsync()
        {
            var employees = new List<EmployeeModel>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            EmployeeModel employee = new EmployeeModel
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                DateOfBirth = reader["DateOfBirth"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["DateOfBirth"]),
                                DateOfJoining = reader["DateOfJoining"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["DateOfJoining"]),
                                Email = reader["Email"].ToString(),
                                Picture = reader["Picture"].ToString()
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }

        private void InsertEmployeeToDatabase(string name, DateTime? dateOfBirth, string email, string pictureFileName, DateTime? DateOfJoining)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("sp_InsertEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfJoining", DateOfJoining ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Picture", pictureFileName);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateEmployeeInDatabase(int id, string name, DateTime? dateOfBirth, string email, string pictureFileName, DateTime? DateOfJoining)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("sp_UpdateEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    command.Parameters.AddWithValue("@DateOfJoining", DateOfJoining);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Picture", pictureFileName);

                    command.ExecuteNonQuery();
                }
            }
        }

        private bool DeleteEmployeeFromDatabase(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("sp_DeleteEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
