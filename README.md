# Football Console App

# This application retrieves and manages football data through a console interface.
# It supports fetching player and league data from an API, saving them to a database, and displaying them in the console.

## JSON Data Structure

# Player Data Structure
{
  "PlayerData": [
    {
      "Id": 0,                      # integer
      "TransferId": null,           # integer or null
      "PlayerId": null,             # integer or null
      "TeamId": null,               # integer or null
      "PositionId": null,           # integer or null
      "DetailedPositionId": null,   # integer or null
      "Start": "YYYY-MM-DD",        # string (date format)
      "End": "YYYY-MM-DD",          # string (date format) or null
      "Captain": false,             # boolean
      "JerseyNumber": null          # integer or null
    }
  ]
}

# League Data Structure
{
  "League": [
    {
      "Id": 0,                      # integer
      "SportId": 0,                 # integer or null
      "CountryId": 0,               # integer or null
      "Name": "",                   # string
      "Active": false,              # boolean
      "ShortCode": "",              # string or null
      "ImagePath": "",              # string or null
      "Type": "",                   # string or null
      "SubType": "",                # string or null
      "LastPlayedAt": "YYYY-MM-DD", # string (date format) or null
      "Category": "",               # string or null
      "HasJerseys": false           # boolean
    }
  ]
}

# Menu Options:
# 1. Retrieves all team members from the API, saves them to the database, and then fetches and displays the data in the console.
# 2. Does the same thing as option 1 but for league data.
# 3. Retrieves the league corresponding to the ID you provide.

# Test Case Descriptions:

# ProcessDataAsync_Should_Add_New_Player
# Purpose: This test checks if a new player is correctly added to the database when it doesn't already exist.
# Process:
# 1. Mocks the API response with player data that doesn't exist in the database.
# 2. Verifies that the player is added and that changes are saved to the database.

# ProcessDataAsync_Should_Not_Add_Existing_Player
# Purpose: This test verifies that the service does not add a player if that player already exists in the database with the same ID and jersey number.
# Process:
# 1. Mocks the API response with player data that matches an existing record in the database.
# 2. Ensures that no new record is added, and no database changes are saved.

# ProcessDataAsync_Should_Add_Multiple_Players
# Purpose: This test ensures that multiple new players are added to the database if they don't already exist.
# Process:
# 1. Mocks the API response with multiple player records.
# 2. Verifies that all new players are added in a batch and that changes are saved.

# ProcessDataAsync_Should_Add_New_League
# Purpose: This test ensures that a new league is correctly added to the database when it doesn't already exist.
# Process:
# 1. Mocks the API response with league data.
# 2. Verifies that the league is added and that changes are saved.

# ProcessDataAsync_Should_Return_False_When_No_Data
# Purpose: This test checks the service's behavior when the API returns no data.
# Process:
# 1. Mocks an API response with an empty data array.
# 2. Ensures that the method returns false, indicating that no data was processed.

# ProcessDataAsync_Should_Add_Log_On_Error
# Purpose: This test verifies that an error during data processing is correctly logged.
# Process:
# 1. Simulates a database error when trying to add a league.
# 2. Verifies that the error is caught, a log entry is created, and the method returns false.

# ProcessDataAsync_Should_Handle_Missing_League_Fields
# Purpose: This test ensures that the service can handle cases where some fields in the league data are missing.
# Process:
# 1. Mocks the API response with incomplete league data (missing SportId and CountryId).
# 2. Verifies that the league is still added and that changes are saved.

# ProcessDataAsync_Should_Return_False_On_Invalid_Json
# Purpose: This test checks how the service handles invalid JSON data.
# Process:
# 1. Mocks an API response with an invalid JSON structure.
# 2. Ensures that the method returns false, indicating that the JSON could not be processed.

# ProcessDataAsync_Should_Return_False_When_League_Detail_Is_Null
# Purpose: This test verifies that the service correctly handles a null response for league details.
# Process:
# 1. Mocks an API response with a null value for the league data.
# 2. Ensures that the method returns false.

# ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Player
# Purpose: This test ensures that a player can be added, removed, and then added again without issues.
# Process:
# 1. Adds a player to the database.
# 2. Removes the player from the database.
# 3. Attempts to add the same player again.
# 4. Verifies that all operations are executed correctly and that the player can be re-added after removal.

# ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Multiple_Players
# Purpose: Similar to the previous test, but for multiple players. This ensures that multiple players can be added, removed, and re-added correctly.
# Process:
# 1. Adds multiple players to the database.
# 2. Removes the players.
# 3. Attempts to add the same players again.
# 4. Verifies that all players are processed correctly in batch operations.

# Unsolved Problem:
# When I delete data from my local database while the program is running, a tracking error pops up. I tried AsNoTracking and Attach methods, but they didn't work. If you can solve this, please contact me.
# Error: The instance of entity type 'PlayerData' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.
