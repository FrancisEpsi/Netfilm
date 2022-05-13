#SCRIPT SQL POUR LA CREATION DE VOTRE PROPRE BASE DE DONNEE
ℹ️ Prenez en compte cette démarche uniquement si nos services (API + BDD) ne sont plus héberger

Le script inclus dans ce dossier permet de créer les tables de la base de donnée "netfilm" et d'ajouter des données à ces tables.

##Prenez note qu'il est nécessaire au préalable de faire ces étapes avant l'importation du script:
- Installer un moteur de base de donnée mariaDB (nous avons utiliser MySQL)
- Créer une base de donnée intitulé "netfilm"
- Enfin, éxécuter le script avec un utilisateur ayant les privilèges de créer des tables / supprimer des tables et insérer des données.

##Pour que l'API (que vous executez) puisse accéder à votre base de donnée:
- Créer un utilisateur "backend" avec le mot de passe "dotnet33" ayant l'accès complet à la base de donnée "netfilm"
- Modifier le port d'écoute de votre moteur de base de donnée sur le port 21832