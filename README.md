# RugbyUnionApi
## Web api for managing a rugby union

#### This is a ASP.NET Core minimal api.

#### The project can be run by opening the RugbyUnionApi.sln file in Visual studio and running the code from there.

#### The api can be explored using a api platform such as PostMan


# API Documentation

#### Api calls
`Get /players` Returns a list of all players

`Get /players/{id}` Returns player with specified ID

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

### Get Single Player
