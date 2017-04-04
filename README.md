# Riot_Api_Datamining
Using the Riot API in C# to extract interesting match and summoner information

### This project is part of the graduate course CPSC 672 - Fundamentals of Social Network Analysis and Data Mining, with (Dr. Rokne)[http://pages.cpsc.ucalgary.ca/~rokne/]

## Project Content
This project contains a few examples of how I use the (RiotSharp)[https://github.com/BenFradet/RiotSharp] library for C# to obtain information about summoner's matches and extract additional infromation from those matches.
It is a simple Windows Forms program with 2 buttons - get a match list for the patch 7.6 for a given summoner and a second button to extract all the information about those matches.

This information is later parsed into a MySQL database and WEKA is used to explore different machine learning algorithms on it.
