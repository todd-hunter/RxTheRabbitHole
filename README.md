# README #

## This Repository contains RX I am experimenting with and/or RX code I want to share (probably with myself!). ###
2018-ish

### Back off Retry 
This code is 'Experimental' for a reason, please beware of inner-dragons.
I was trying to highlight the difficulty with 'Back Off Retry' in an RX discussion where previous solutions I'd seen used recursion and had blown stack issues.
#### Trains
Watching the tube/train arrive that evening and it was out of service, again... It occurred to me you could probably use observables that contain inner-observables to avoid the recursion problem?

- You can ignore the out of service trains and wait for a train that arrives that isn't broken.
- The tunnel is the outer observable and the trains ride along, and they are observable streams as well.
- A train/stream yielding OK is treated as the infinite inner observable (zero, many, or an infinite number of people getting out)
- An out-of-service train(stream yields error)is also just another broken tube train, get off it and wait for the next one.
- I never found this one easy... And the code is, sadly, still way to complicated. 
  https://github.com/todd-hunter/RxTheRabbitHole/blob/master/RxInWonderland/Rx.Experimental/BackoffExtensions.cs

   *** I'd suggest just sticking with .Retry(1) and keeping life easy. ***




