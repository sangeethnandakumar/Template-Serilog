# Express.Logging

Express.Logging wraps the NLogger mechanism to expose simple text based logging APIs

### Versions

Released verisons are listed below

| Version | Change log |
| ------ | ------ |
| 1.1 | FFin.Ecnription 1.1, Microsoft.AES 1.1 |



### Usage
Log a line
```csharp
ExpressLog.Debug("Text to be logged");
ExpressLog.Info("Text to be logged");
ExpressLog.Error("Text to be logged");
...
```
Log an object
```csharp
ExpressLog.Info("Look at this object", paymentPayload);
```
