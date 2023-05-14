For demonstrate purposes only, I moved selected Blazor Pages and Models from 
EDennis.AspNetUtils.Tests.Blazor.Sample.BS to
EDennis.AspNetUtils.Tests.Blazor.Sample.Shared.

To reference Blazor Pages from a shared Razor Components Library, ensure that 
your Router includes the shared Assembly via AdditionalAssemblies.
(see https://stackoverflow.com/a/64305929)