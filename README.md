# NET FILM
Projet pour l'EPSI réalisé par François SAURA et Loïc LABAISSE

⚠️ Pour que vous puissez tester notre travail, nous avons choisi d'héberger le site internet, la base de donnée et l'API PENDANT 1 SEMAINE SEULEMENT. ⚠️

## TESTS avec solution hébergée:
http://netfilm.saufrasoft.fr/

ℹ️ Vous n'avez pas à vous soucier de la partie "Configuration de l'environnement de test" si le site ci-dessus fonctionne correctement.

***
Compte utilisateur déja existant sur le site internet (pour tester):

Email : titouan@titouan.com , mdp :titouan ;

Email : charlotte@charlotte.com , mdp :charlotte ;

Email: francois@francois.com, mdp : francois;


## Configuration de l'environnement de test:

Dans cette partie, nous verrons comment déployer vous même notre projet afin de le tester chez vous (dans le cas ou nous ne l'hébergons plus)

### Installation Frontend (REACT)

ℹ️ Vous devez mettre un fichier .env dans /front avec pour attribut REACT_APP_API_MOVIE=http://netfilm33.duckdns.org:8080/ si l'API est encore héberger par nos moyens sinon mettre REACT_APP_API_MOVIE=http://127.0.0.1:8080/ si l'API est en cours d'éxécution sur la même machine que le serveur WEB ou sera le front.
```
$ git clone https://github.com/FrancisEpsi/BackendMovieWebsite.git
$ cd Front/front
$ npm install
$ npm start
```

Vous vous trouverez alors avec un web server de développement hébergé sur votre boucle locale au port 3000

### Base de donnée

Veuillez lire le README.md dans Database/README.md

Vous trouverez le script pour la création des tables et des données à l'emplacement suivant: Database/dump-netfilm-202205131727.sql

### Installation Backend (C#.NET)

ℹ️ Si vous n'êtes pas sous Windows, nous vous recommandons d'installer une VM Windows pour éxécuter l'API (plus simple).

⚠️ L'API est configurée pour se connecter à une base de donnée MySQL locale sur le port 21832 nommée "netfilm" avec l'utilisateur "backend" et le mot de passe "dotnet33"

#### Sous windows (version déjà compilée):

Exécuter en tant qu'administrateur:
```
$ Build\API\ExoHttpAPI.exe
```
L'API est à l'écoute de requête sous le port 8080

#### Sous Linux (version à compiler)

Il est également possible de compiler le projet pour Linux x86 mais pour cela il faudra convertir le projet sous le framework .NET CORE. Il existe des tutoriels prévu à cet effet sur internet.


## Structure du repository:

Build/WebRoot/ : Contient les fichiers à placer à la racine d'un serveur WEB

Build/API/ : Contient une version compilée pour Windows de l'API

Database/ : Contient le script nécessaire et les instructions pour la création d'une base de donnée netfilm

ExoHttpAPI.sln : Solution à ouvrir avec Visual Studio (Pas le vscode) permettant d'accéder au code source de l'API en C# (classes et méthodes dans ExoHttpAPI/)

Front/ : Contient le projet React complet du Frontend

ℹ️ N'hésitez pas à lire les fichiers .md dans les sous dossiers du repository
