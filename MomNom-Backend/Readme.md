# Momnom-Backend

## Installation
dotnet restore

dotnet tool install --global dotnet-ef

- create/update EF model (after updating db tables/field):
- dotnet ef dbcontext scaffold "name=DefaultConnection" "Pomelo.EntityFrameworkCore.MySql" -o Model/Db -c MomNomContext --force
