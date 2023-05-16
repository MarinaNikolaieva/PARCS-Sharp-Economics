# PARCS-Sharp-Economics

Imagine yourself a country. The country has its natural resources and wildlife, and it also has the ability to aquire them for itself.

Each piece of resource has its effect on the country's economic potential. And each piece of resource goes to some Industry.

Industries have three different levels.

Level 1 has its needed resources independently - if at least one is present, the industry can progress. But if the industry has required resources (marked by !), if these are not present, the industry can't progress. The resources are only the natural/wildlife/global resources.

Level 2 needs all of its resources, both needed and required, in order to develop. If at least one isn't present, the industry can't progress. The resources may be Level-1 industries.

Level 3 is the same as Level 2, but it needs Level-2 industries to progress.

There may be multiple countries on the world map. Calculating the potential of each one sequentially will be way too long. But all countries may be calculated in parallel using threads and sped up with the usage of distributed calculations and PARCS system.
