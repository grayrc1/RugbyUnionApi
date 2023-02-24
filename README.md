# RugbyUnionApi
## Web api for managing a rugby union

#### This is a ASP.NET Core minimal api.

#### The project can be run by opening the RugbyUnionApi.sln file in Visual studio and running the code from there.

#### The api can be explored using a api platform such as PostMan


# API Documentation

#### Api calls
`Get /players` Returns a list of all players

`Get /players/{id}` Returns player with specified ID

`Get /players/playerteam/{id}` Returns the team of player with specified ID

`Get /players/getage/{age}`
Returns all of the players with the specified age

`Post /players` Creates new player

`Put /players/{id}` Updates player with specified ID

`Delete /players/{id}` Deletes player with specified ID

`Get /teams` Returns a list of all teams

`Get /teams/{id}` Returns team with speicified ID

`Get /teams/getcoachplayers{coachname}` Returns all player who have the specified coach

`Post /teams` Creates new team

`Put /teams/{id}` Updates team with specified ID

`Delete /teams/{id}` Deletes team with specified ID


## Players

### Get all players

`GET /players`

Returns a list of all players.

#### Response

- `200 OK` on success
- `404 Not Found` if no players found

#### Example

```json
[
  {
    "id": 1,
    "name": "John Doe",
    "birthDate": "1990-01-01T00:00:00Z",
    "height": 180,
    "weight": 75,
    "placeOfBirth": "New York City",
    "teamId": 1
  },
  {
    "id": 2,
    "name": "Jane Smith",
    "birthDate": "1995-05-05T00:00:00Z",
    "height": 170,
    "weight": 60,
    "placeOfBirth": "Los Angeles",
    "teamId": null
  }
]
```

### Get Single Player
`GET /players/{id}`

Returns a list of all players.

#### Response

- `200 OK` on success
- `404 Not Found` if no players found

#### Example

```json
{
  "id": 1,
  "name": "John Doe",
  "birthDate": "1990-01-01T00:00:00Z",
  "height": 180,
  "weight": 75,
  "placeOfBirth": "New York City",
  "teamId": 1
}
```

### Create Player
`POST /players`

Creates a new player in the system.

### Data Params:

The request body should contain a JSON object representing the new player to create, with the following properties:

- `name`: (string) The player's name (required)
- `birthDate`: (string, format: YYYY-MM-DD) The player's date of birth (required)
- `height`: (integer) The player's height in centimeters (required)
- `weight`: (integer) The player's weight in kilograms (required)
- `placeOfBirth`: (string) The player's place of birth (optional)
- `teamId`: (integer) The ID of the team the player is signed with (optional)

### Success Response:

- **Code:** 201 CREATED<br />
  **Content:** Created player

### Error Responses:

- **Code:** 400 BAD REQUEST<br />
  **Content:** An error message indicating that one or more required fields are missing or invalid.
  
- **Code:** 500 INTERNAL SERVER ERROR<br />
  **Content:** An error message indicating that the server encountered an unexpected error while processing the request.

### Example Request:
```json
POST /players/ HTTP/1.1
Content-Type: application/json

{
  "name": "John Doe",
  "birthDate": "1990-01-01T00:00:00Z",
  "height": 180,
  "weight": 75,
  "placeOfBirth": "New York City",
  "teamId": 1
}
```

### Example Response:
```json
HTTP/1.1 201 Created
Content-Type: application/json
{
  "name": "John Doe",
  "birthDate": "1990-01-01T00:00:00Z",
  "height": 180,
  "weight": 75,
  "placeOfBirth": "New York City",
  "teamId": 1
}
```
