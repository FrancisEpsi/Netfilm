# API C#

## Routes

Chacunes des retourne un JSON contenant des informations ou des données.
En cas d'erreur, les détails de l'erreur sont souvent inscrit dans le json à la clé "comment"

### GET /
Page d'accueil de l'API

### GET /FILMS/<userID>
Retourne un HTTP 200 avec un JSON contenant une liste de film.
Chacun des films à une clé "user_liked" définit par l'API qui indique si l'utilisateur à aimer ou pas ce film.

- Peut retourner HTTP 400 si l'userID fourni n'est pas un entier
- Peut retourner 500 si il est impossible de joindre la base de donnée, ou si la liste des films reçu par TheMovieDatabase est vide

### GET /LOGIN/<email>/<passhash>
avec
<email> : Adresse e-mail de l'utilisateur
<passhash> : Mot de passe de l'utilisateur haché avec l'algorythme SHA256 

Vérifie si les identifiants de l'utilisateur sont correct

- Retourne un HTTP 200 avec un JSON contenant les informations sur l'utilisateur (avec userID, firstname, lastname, email)
- Retourne HTTP 400 avec un JSON si l'email et/ou le mot de passe sont incorrect
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si tous les arguments demandés (<email>, <passhash>) ne sont pas fournies

### GET /REGISTER/<firstname>/<lastname>/<email>/<passhash>
avec
<firstname> : Le prénom de l'utilisateur
<lastname> : Le nom de famille de l'utilisateur
<email> : L'adresse e-mail utilisée pour la connexion de l'utilisateur
<passhash> : Le hash SHA256 du mot de passe de l'utilisateur utilisé pour sa connexion

Enregistre dans la base de donnée un nouvel utilisateur.
Vérifie avant tout qu'il n'existe pas déjà (grâce à son e-mail).

- Retourne HTTP 200 avec un JSON détaillant l'opération si l'utilisateur à bien été créer
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si tous les arguments demandés ne sont pas fournies dans l'URI
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si un des arguments est vide
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si l'API n'arrive pas à se connecter à la base de donnée
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si  l'API n'arrive pas à vérifier si l'utilisateur n'existe pas déjà en base de donnée
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si  l'API détecte qu'un utilisateur ayant la même adresse e-mail existe déjà en base de donnée
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si  l'API n'arrive pas à effectuer une insertion des données de l'utilisateur en base de donnée

### GET /SETLIKE/<userID>/<filmID>
avec
<userID> : L'identifiant de l'utilisateur connecté qui souhaite déposé un j'aime
<filmID> : L'identifiant TheMovieDatabase d'un film que l'utilisateur souhaite aimé

Permet de définir qu'un utilisateur aime un film.
Vérifie avant tout que l'utilisateur ne l'a pas déjà aimer

- Retourne HTTP 200 avec un JSON contenant la liste des films pour cet utilisateur avec le j'aime ajouté
- Retoune HTTP 200 avec un JSON détaillant l'opération si l'utilisateur à déjà aimer ce film.
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si <userID> n'est pas un entier
- Peut retourner HTTP 500 avec un JSON détaillant l'erreur si une erreur internet s'est produite (absence des films de TheMovieDatabase, Impossible de se connecter à la base de donnée)
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si l'insertion dans la base de donnée de la mention j'aime à échouée

### GET /UNSETLIKE/<userID>/<filmID>
avec
<userID> : L'identifiant de l'utilisateur connecté qui souhaite enlever un j'aime
<filmID> : L'identifiant TheMovieDatabase d'un film que l'utilisateur souhaite ne plus aimer

Permet de dire qu'un utilisateur n'aime plus un film
Vérifie avant tou que l'utilisateur ne l'a pas déjà dislike.

- Retourne HTTP 200 avec un JSON contenant la liste des films pour cet utilisateur avec le j'aime enlevé
- Retoune HTTP 200 avec un JSON détaillant l'opération si l'utilisateur n'aime déjà pas ce film
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si <userID> n'est pas un entier
- Peut retourner HTTP 500 avec un JSON détaillant l'erreur si une erreur internet s'est produite (absence des films de TheMovieDatabase, Impossible de se connecter à la base de donnée)
- Peut retourner HTTP 400 avec un JSON détaillant l'erreur si la suppresion en base de donnée de la mention j'aime à échouée

## Routes obscolètes / non ou plus utilisée

### GET /FILMS
Retourne un JSON contenant une liste de film

### GET /LIKE/<userID>/<filmID>/<liked>
avec
<userID> : L'identifiant de l'utilisateur connecté qui souhaite enlever un j'aime
<filmID> : L'identifiant TheMovieDatabase d'un film que l'utilisateur souhaite ne plus aimer
<liked> : une valeure indiquant TRUE si un j'aime doit être déposé (équivalent à /SETLIKE) ou FALSE si un j'aime doit être retiré (équivalent à /UNSETLIKE)

### POST /POSTEST
Route utilisée à des fin de test qui permettant l'acquisition du body dans une requête POST

### PUT /PUTLIKE
Abandoné car ne fonctionne plus du jours au lendemain à cause de CORS.
Servait à définir un j'aime en passant des paramètres (formatté en JSON) dans le body (<userID>,<filmID>, et <liked>)
Remplacé par /SETLIKE et /UNSETLIKE

### POST /POSTLOGIN
Abandoné car ne fonctionne plus du jours au lendemain à cause de CORS.
Aurait servit à remplacer /LOGIN par une requête POST. Ce qui aurait éviter de faire transiter les informations de connexion de l'utilisateur au travers de l'URL par une requête GET (bien qu'il ne puisse pas la voir car c'est une requête fait en JS, pas une redirection, cette pratique n'est jamais utilisée)

### POST /GETUSERLIKES
Aurait permis d'obtenir la liste des films aimé par l'utilisateur
Abandoné car pas le temps de l'intégrer au front et même en backend je ne sais plus si j'avais terminée cette fonction.
De toute façon si on aurait eu le temps, on n'aurait pas fait une route mais loïc aurait pu directement filtrer l'affichage de la liste de film avec uniquement ceux qui ont été liké par l'utilisateur
Abandonné encore plus à cause du CORS