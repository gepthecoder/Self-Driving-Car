# Self-Driving-Car

🚗🚗  https://g3pgames.itch.io/self-driving  🚕🚕


Practice: Example Of Using Neural Networks and Genetic Algorithms On Self Driving Cars
#
# Neural Networks & Genetic Algorithms
#
Neural network takes in the inputs and gives out the outputs. Genetic Algorithm trains the neural network on how to get better outputs.
Car has 3 major sensors (in our case): three extruded raycasts in each direction - to hit obsticles.
The output of these sensors is the distance of the wall, which is fed into the neural network.
The neural network manipulates these values and does a bunch of functions on them.
#
Two outputs are given out: 
- STEERING, 
- ACCELARATION -> Which controls the car.
#
In order to train the car better we need the GENETIC ALGORITHM.
#
Program Logic:

50 random cars are generated that has a random neural network (each interprete the sensory data in a different way and do random stuff)
When a car "dies" meaning in hit an obsticle, we select the ones that did the best and we combine (morph) them together in order to 
create a slightly more efficient car.
#
Example:
- We pick parent A and parent B
- We take a few weights from both and merged them together to create a child.
- The child "should" then do better the the generation before, so we pick 20 best cars from 50 randomly generated ones.
- Then we generate around 50 more cars using the top 20 parents.

Other factors that influence outcome:
- Mutation Rates
- Selecting the worst performers
