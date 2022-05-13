# NET FILM
Projet pour l'EPSI réalisé par François SAURA et Loïc LABAISSE

⚠️ Pour que vous puissez tester notre travail, nous avons choisi d'héberger la base de donnée et l'API PENDANT 1 SEMAINE SEULEMENT. ⚠️

## Installation REACT

ℹ️ Vous devez mettre un fichier .env avec pour attribut REACT_APP_API_MOVIE=http://netfilm33.duckdns.org:8080/ si l'API est encore héberger par nos moyens sinon mettre REACT_APP_API_MOVIE=http://127.0.0.1:8080/
```
$ git clone https://github.com/FrancisEpsi/BackendMovieWebsite.git
$ cd Front/front
$ npm install
$ npm start
```


## Installation C#.Net (Backend)

ℹ️ Cette étape est uniquement utile dans le cas ou nous n'hébergeons plus l'API ainsi que la base de donnée.
Il sera alors nécessaire d'éxécuter l'API (précompilée) puis créer une base de donnée MySQL locale avec la même structure.

### Sous windows (version déjà compilée):

Exécuter en tant qu'administrateur:
```
$ ExoHttpAPI\bin\Debug\net5.0\ExoHttpAPI.exe
```

L'API est à l'écoute de requête sous le port 8080

⚠️ Information ⚠️

L'API est configurée pour se connecter à une base de donnée MySQL locale sur le port 21832 nommée "netfilm" avec l'utilisateur "backend" et le mot de passe "dotnet33"
Si jamais nous n'hébergont plus l'API et la base de donnée lors de vos test, veuillez recréer la base de donnée avec les scripts SQL fournis puis ajouter manuellement l'utilisateur "backend" avec le mot de passe "dotnet33" avec tous les droits sur la base "netfilm".
La base de donnée devra être sur la même machine que l'API, en écoute sur le port 21832.

***
Compte déja existant 

Email : titouan@titouan.com , mdp :titouan ;
Email : charlotte@charlotte.com , mdp :charlotte ;
Email: francois@francois.com, mdp : francois;
