# API C#

:warning: Si vous souhaitez recompiler l'API, veuillez faire attention à vous fournir une clé d'API auprès de TheMovieDatabase puis de la placé dans la variable "TMDB_API_KEY" dans "Program.cs". Pour information, la version précompilé dans /Build/API/ExoHttpAPI.exe contient déjà la clé d'API de TheMovieDatabase pour vous facilier la tâche et vous éviter de compiler l'API.

## Informations générales
Langage de programmation: C#

Framework utilisé: .NET Framework 5

Type de projet: Application Console

IDE utilisé: Microsoft Visual Studio Community (Gratuit). Recommandé pour profiter d'Intellisense, faciliter de compilation en un clic (sans ligne de commande)

Bien que ce n'est pas vraiment adapté comme Meteor, Django ou symfony, J'ai choisi de faire cette API en C#.NET car c'est un langage de programmation que je connais très bien. Cela permet également de controller de manière plus bas niveau les différents échanges entre le front et le back. Je préfère cette approche plutôt que d'utiliser un framework qui fait tout à notre place, sans vraiment savoir comme ça fonctionne derrière. Cela était un défi pour moi aussi car quand nous faisons des projets avec Loïc (développeur WEB, spécialisé dans le front) je n'étais presque d'aucune utilité avec les technos que je métrise pour le développement d'un site WEB avec lui. On a donc appris aux travers de ce projet à accorder nos technologies pour pouvoir travailler enssemble sur un projet WEB.

## Fonctionnement
Le projet est basé sur la classe HTTP Listenner du framework.

C'est une classe basé sur les sockets qui est concu pour la communication aux travers du protocole HTTP.
Le socket est à l'écoute de connexion entrantes, je définis des routes, je peux facilement intercepter les connexions entrantes, récupérer les requetes cliente, les traités, puis retourner une réponse.


## Détails des classes

### Program.cs
Classe principale du programme. La méthode Main() est celle éxécuté dès le lancement de l'API.

C'est dans cette classe que sont traités les connexions clientes, les routes, les requêtes etc...
### DatabaseInterface.cs
C'est la classe permettant de faciliter la communication avec la base de donnée, et pour éviter le code répétitif.

Elle contient des propriétées pour définir le connexionString de la base de donnée (identifiant de connexion aux moteur de base de donnée). De cette sorte, il est facile de changer l'emplacement de la base de donnée en modifier ces propriétées.

J'y met des fonctions permettant de faire de la récupération de donnée SQL ou l'éxécution de requêtes sur la BDD.
### JsonModels.cs
Ce fichier contient plusieurs classes.

Ces classes servent de modèle pour la sérialisation JSON.

Etant donner que les données transmises entre le client WEB et l'API sont formatté en JSON, j'ai créer des classes pour chaque requête respectant un modèle JSON particulier (en fonction de la requête).

Cela me permet de désérialiser les données que m'envoit Loïc en Frontend afin que je puisse les lire et les traités facilement grâce à la programmation orienté objet. Je peux également les sérialisés une fois le traitement effectué pour les renvoyer à Loïc.

