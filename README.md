### Convert Binary Tree Lambda Expression to SQL query
### Tech: C#/.NET
This project is built to convert Lambda Expression Binary Tree to SQL query.\
I hope it can be of help to you.\
Author: Man Pham

### Example:
```csharp
var employeeQuery = new GenerateSQL<Employee>();
//where
Expression<Func<Employee, bool>> where1 = t => t.id == 1 || t.name == "Minh" && t.age == 21;
Expression<Func<Employee, bool>>[] where2 = new Expression<Func<Employee, bool>>[]
{
  e=>e.age==21 && e.id==1,
  e=>e.name=="Man" || e.name == "Pham"
};
//select
Expression<Func<Employee, Employee>> select1 = e => new Employee { id = e.id, age = e.age };
Expression<Func<Employee, Employee>> select2 = e => new Employee { age = e.age, name = e.name };


var query1 = employeeQuery.GenerateQuery(where1);
var query2 = employeeQuery.GenerateQuery(where2);
var query3 = employeeQuery.GenerateQuery(where1, select1);
var query4 = employeeQuery.GenerateQuery(where1, select2);
var query5 = employeeQuery.GenerateQuery(where2, select1);
var query6 = employeeQuery.GenerateQuery(where2, select2);

var query7 = employeeQuery.GenerateQuery(x => x.id == 1 || x.id == 2 && x.name == "Man", x => new Employee { name = x.name, id = x.id });
var query8 = employeeQuery.GenerateQuery(x=>x.name=="Man", select2);

var query9 = employeeQuery.ToQuery();

var query10 = employeeQuery.Where(where1).Select(select1).ToQuery();
var query11 = employeeQuery.Where(where1).Select(select2).ToQuery();
var query12 = employeeQuery.Where(where2).Select(select1).ToQuery();
var query13 = employeeQuery.Where(where2).Select(select2).ToQuery();

var query14 = employeeQuery.Where(where1).ToQuery();
var query15 = employeeQuery.Where(where2).ToQuery();

var query16 = employeeQuery.Select(select1).ToQuery();
var query17 = employeeQuery.Select(select2).ToQuery();

var query18 = employeeQuery.Where(where1).Where(where2).Select(select1).Select(select2).ToQuery();
var query19 = employeeQuery.Where(where1).Where(where2).Select(select2).Select(select1).ToQuery();
var query20 = employeeQuery.Where(where2).Where(where1).Select(select1).Select(select2).ToQuery();
var query21 = employeeQuery.Where(where2).Where(where1).Select(select2).Select(select1).ToQuery();

var query22 = employeeQuery.Where(x => x.id == 1 && x.name == "Man").Select(x => new Employee { name = x.name, age = x.age }).ToQuery();
var query23 = employeeQuery.Where(x => x.id == 1).Where(x => x.name == "Man").Select(x => new Employee { name = x.name, age = x.age }).Select(x => new Employee { name = x.name, id = x.id }).ToQuery();

Console.WriteLine("query1: " + query1);
Console.WriteLine("query2: " + query2);
Console.WriteLine("query3: " + query3);
Console.WriteLine("query4: " + query4);
Console.WriteLine("query5: " + query5);
Console.WriteLine("query6: " + query6);
Console.WriteLine("query7: " + query7);
Console.WriteLine("query8: " + query8);
Console.WriteLine("query9: " + query9);
Console.WriteLine("query10: " + query10);
Console.WriteLine("query11: " + query11);
Console.WriteLine("query12: " + query12);
Console.WriteLine("query13: " + query13);
Console.WriteLine("query14: " + query14);
Console.WriteLine("query15: " + query15);
Console.WriteLine("query16: " + query16);
Console.WriteLine("query17: " + query17);
Console.WriteLine("query18: " + query18);
Console.WriteLine("query19: " + query19);
Console.WriteLine("query20: " + query20);
Console.WriteLine("query21: " + query21);
Console.WriteLine("query22: " + query22);
Console.WriteLine("query23: " + query23);
```
Output will be:
```sql
query1: SELECT * FROM Employee WHERE id='1' or name='Minh' and age='21'
query2: SELECT * FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query3: SELECT id, age FROM Employee WHERE id='1' or name='Minh' and age='21'
query4: SELECT age, name FROM Employee WHERE id='1' or name='Minh' and age='21'
query5: SELECT id, age FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query6: SELECT age, name FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query7: SELECT name, id FROM Employee WHERE id='1' or id='2' and name='Man'
query8: SELECT age, name FROM Employee WHERE name='Man'
query9: SELECT * FROM Employee
query10: SELECT id, age FROM Employee WHERE id='1' or name='Minh' and age='21'
query11: SELECT age, name FROM Employee WHERE id='1' or name='Minh' and age='21'
query12: SELECT id, age FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query13: SELECT age, name FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query14: SELECT * FROM Employee WHERE id='1' or name='Minh' and age='21'
query15: SELECT * FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham'
query16: SELECT id, age FROM Employee
query17: SELECT age, name FROM Employee
query18: SELECT id, age, name FROM Employee WHERE id='1' or name='Minh' and age='21' and age='21' and id='1' and name='Man' or name='Pham'
query19: SELECT age, name, id FROM Employee WHERE id='1' or name='Minh' and age='21' and age='21' and id='1' and name='Man' or name='Pham'
query20: SELECT id, age, name FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham' and id='1' or name='Minh' and age='21'
query21: SELECT age, name, id FROM Employee WHERE age='21' and id='1' and name='Man' or name='Pham' and id='1' or name='Minh' and age='21'
query22: SELECT name, age FROM Employee WHERE id='1' and name='Man'
query23: SELECT name, age, id FROM Employee WHERE id='1' and name='Man'
```
