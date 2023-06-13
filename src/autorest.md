Options [found here](https://github.com/Azure/autorest.csharp/blob/feature/v3/src/AutoRest.CSharp/Common/AutoRest/Plugins/Configuration.cs)

```yaml
input-file: https://api.example.com/swagger/v1/swagger.json
namespace: AutoRestExample
model-namespace: false
license-header: NONE
output-folder: $(this-folder)/Generated
clear-output-folder: true
csharp: true
public-clients: true
skip-csproj: true
generation1-convenience-client: true
override-client-name: ExampleApi
```