# Football_ConsoleApp
# Football_ConsoleApp
```json
{
  "PlayerData": [
    {
      "Id": 0,                      // integer
      "TransferId": null,           // integer or null
      "PlayerId": null,             // integer or null
      "TeamId": null,               // integer or null
      "PositionId": null,           // integer or null
      "DetailedPositionId": null,   // integer or null
      "Start": "YYYY-MM-DD",        // string (date format)
      "End": "YYYY-MM-DD",          // string (date format) or null
      "Captain": false,             // boolean
      "JerseyNumber": null          // integer or null
    }
  ]
}

{
  "League": [
    {
      "Id": 0,                // integer
      "SportId": 0,           // integer or null
      "CountryId": 0,         // integer or null
      "Name": "",             // string
      "Active": false,        // boolean
      "ShortCode": "",        // string or null
      "ImagePath": "",        // string or null
      "Type": "",             // string or null
      "SubType": "",          // string or null
      "LastPlayedAt": "YYYY-MM-DD",  // string (date format) or null
      "Category": "",         // string or null
      "HasJerseys": false     // boolean
    }
  ]
}
```

Unsolved Problem : 

When I delete datas from my local Database while Program running Tracking error pops up I tried AsNoTracking and Attach  Metots it also didin't work if u can solve it pls contact wtih me.
The log when I did that : 
![image](https://github.com/user-attachments/assets/f21c94e1-8e90-4657-896b-8f1966f6102c)
Error : The instance of entity type 'PlayerData' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.

The app when I did that : 

![image](https://github.com/user-attachments/assets/fae7686f-baad-47f8-8307-096626592f06)

