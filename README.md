# Seatours

To install run

git clone https://github.com/Vettvangur/EimskipTravel.git --recurse

If you forget to --recurse on clone use -> git submodule update --init --recursive

or if you have already cloned the project, run
git submodule update --init
to initialize the submodule

To build, run the following in two seperate shells:

gulp

webpack

or

webpack --env.prod

for production

Also, when switching between test and production bokun api:

Change web.config api keys
and change PriceGroup constants
Umbraco.Frontend/js/data/constants.ts
