# NET FILM
Projet pour l'EPSI réalisé par François SAURA et Loïc LABAISSE

⚠️ Pour que vous puissez tester notre travail, nous avons choisi d'héberger le site internet, la base de donnée et l'API PENDANT 1 SEMAINE SEULEMENT. ⚠️

## TESTS avec solution hébergée:
http://netfilm.saufrasoft.fr/

ℹ️ Vous n'avez pas à vous soucier de la partie "TESTS avec solution à héberger par vous même (installation locale)" si le site ci-dessus fonctionne correctement.

## TESTS avec solution à héberger par vous même (installation locale):

### Installation Frontend (REACT)

ℹ️ Vous devez mettre un fichier .env dans /front avec pour attribut REACT_APP_API_MOVIE=http://netfilm33.duckdns.org:8080/ si l'API est encore héberger par nos moyens sinon mettre REACT_APP_API_MOVIE=http://127.0.0.1:8080/ si l'API est en cours d'éxécution sur la même machine que le serveur WEB ou sera le front.
```
$ git clone https://github.com/FrancisEpsi/BackendMovieWebsite.git
$ cd Front/front
$ npm install
$ npm start
```

Vous vous trouverez alors avec un web server de développement hébergé sur votre boucle locale au port 3000


### Installation Backend (C#.NET)

ℹ️ Cette étape est uniquement utile dans le cas ou nous n'hébergeons plus l'API ainsi que la base de donnée.
Il sera alors nécessaire d'éxécuter l'API (précompilée) puis créer une base de donnée MySQL locale avec la même structure.

ℹ️ Si vous n'êtes pas sous Windows, nous vous recommandons d'installer une VM Windows pour éxécuter l'API (plus simple).

⚠️ L'API est configurée pour se connecter à une base de donnée MySQL locale sur le port 21832 nommée "netfilm" avec l'utilisateur "backend" et le mot de passe "dotnet33"

⚠️ Si jamais nous n'hébergont plus l'API et la base de donnée lors de vos test, veuillez recréer la base de donnée avec les scripts SQL fournis puis ajouter manuellement l'utilisateur "backend" avec le mot de passe "dotnet33" avec tous les droits sur la base "netfilm".
La base de donnée devra être sur la même machine que l'API, en écoute sur le port 21832.

#### Sous windows (version déjà compilée):

Exécuter en tant qu'administrateur:
```
$ ExoHttpAPI\bin\Debug\net5.0\ExoHttpAPI.exe
```
L'API est à l'écoute de requête sous le port 8080

#### Sous Linux (version à compiler)

Il est également possible de compiler le projet pour Linux x86 mais pour cela il faudra convertir le projet sous le framework .NET CORE. Il existe des tutoriels prévu à cet effet sur internet.


### Base de donnée
ℹ️ Cette étape est uniquement utile dans le cas ou nous n'hébergeons plus l'API ainsi que la base de donnée.

Veuillez lire le README.md dans Database/README.md

Vous trouverez le script pour la création des tables et des données à l'emplacement suivant: Database/dump-netfilm-202205131727.sql

***
Compte déja existant 

Email : titouan@titouan.com , mdp :titouan ;
Email : charlotte@charlotte.com , mdp :charlotte ;
Email: francois@francois.com, mdp : francois;
