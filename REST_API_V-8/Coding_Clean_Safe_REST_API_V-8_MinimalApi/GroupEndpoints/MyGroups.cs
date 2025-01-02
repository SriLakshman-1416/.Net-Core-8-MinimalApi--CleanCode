namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.GroupEndpoints
{
    public static class MyGroups
    {
        public static RouteGroupBuilder GroupCountries(this RouteGroupBuilder group)
        {
            var countries = new string[]
            {
                "UK",
                "Japan",
                "France"
            };

            var languages = new Dictionary<string, List<string>>()
            {
                {"UK", new List<string>{ "English", "Spanish" } },
                {"Japan", new List<string>{ "English","Japanies"} },
                {"France", new List<string>{ "English", "French" } }
            };

            group.MapGet("/", () => countries);
            group.MapGet("/{id}", (int id) => countries[id]);
            group.MapGet("/{id}/languages", (int id) =>
            {
                var country = countries[id];
                return languages[country];
            });
            return group;
        }
    }
}
