namespace LibplanetConsole.Common;

public static class DependencyUtility
{
    public static IEnumerable<T> TopologicalSort<T>(
        IEnumerable<T> items,
        Func<T, IEnumerable<T>> dependencies)
        where T : class
    {
        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>();

        foreach (var item in items)
        {
            Visit(item, dependencies, sorted, visited);
        }

        return sorted;
    }

    public static IEnumerable<T> GetDependencies<T>(T obj, IEnumerable<T> items)
        where T : class
    {
        var attributes = Attribute.GetCustomAttributes(obj.GetType(), typeof(DependencyAttribute))
                                  .OfType<DependencyAttribute>();
        foreach (var attribute in attributes)
        {
            yield return items.Single(item =>
            {
                return attribute.DependencyType.IsInstanceOfType(item);
            });
        }
    }

    private static void Visit<T>(
        T item,
        Func<T, IEnumerable<T>> getDependencies,
        List<T> sorted,
        Dictionary<T, bool> visited)
        where T : class
    {
        var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

        if (alreadyVisited)
        {
            if (inProcess)
            {
                throw new InvalidOperationException("Cyclic dependencies are not allowed");
            }
        }
        else
        {
            visited[item] = true;

            var dependencies = getDependencies(item);
            if (dependencies is not null)
            {
                foreach (var dependency in dependencies)
                {
                    Visit(dependency, getDependencies, sorted, visited);
                }
            }

            visited[item] = false;
            sorted.Add(item);
        }
    }
}
