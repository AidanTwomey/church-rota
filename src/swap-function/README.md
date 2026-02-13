# Swap Function

Azure Functions app for managing church rota swaps, schedules, and availability.

## Structure

- **Functions/** - HTTP-triggered Azure Functions
  - `ScheduleFunctions.cs` - Schedule CRUD operations
  - `SwapFunctions.cs` - Swap request handling with custom media types
  - `AvailabilityFunctions.cs` - Availability management

- **Models/** - DTOs and Table Storage entities
  - `Dtos.cs` - Request/response DTOs
  - `ScheduleTableEntity.cs` - Schedule table entity
  - `PersonTableEntity.cs` - People table entity
  - `AvailabilityTableEntity.cs` - Availability table entity

- **Repositories/** - Data access layer for Azure Table Storage
  - `IScheduleRepository.cs` / `ScheduleRepository.cs`
  - `IPeopleRepository.cs` / `PeopleRepository.cs`
  - `IAvailabilityRepository.cs` / `AvailabilityRepository.cs`

- **Services/** - Business logic
  - `IReaderMatchingService.cs` / `ReaderMatchingService.cs` - Swap matching algorithm

## API Endpoints

### Schedule
- `GET /api/schedule?startDate={date}&endDate={date}` - Get schedules in date range
- `GET /api/schedule/{scheduleId}` - Get specific schedule entry
- `POST /api/schedule` - Create new schedule entry
- `PUT /api/schedule/{scheduleId}` - Update schedule entry
- `PATCH /api/schedule/{scheduleId}` - Request swap (Content-Type: application/vnd.church-rota.swap-request+json)

### Availability
- `GET /api/availability/{personId}?startDate={date}&endDate={date}` - Get person availability
- `POST /api/availability/{personId}` - Set availability dates

## Local Development

1. Install [Azurite](https://github.com/Azure/Azurite) for local Table Storage:
   ```bash
   npm install -g azurite
   azurite --silent --location /tmp/azurite --debug /tmp/azurite/debug.log
   ```

2. Run the function app:
   ```bash
   cd src/swap-function
   dotnet build
   func start
   ```

## Deployment

The function app is deployed via Terraform (see `devops/main.tf`). The GitHub Actions workflow `deploy-infrastructure.yml` handles deployment.

## Matching Algorithm

The `ReaderMatchingService` finds replacement readers based on:
1. **Role match** - Must have the required role
2. **Availability** - Marked as available (or no availability record)
3. **Not already scheduled** - Not assigned on the same date
4. **Fairness** - Selects person with fewest assignments in last 3 months

If no match is found, returns suggestions of people who will be available soon.
