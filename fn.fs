module fn

open fsharper.fn

let rec concat list = 
    match list with
    | x :: xs  ->  x @ concat xs
    | [] -> []

let rec sames p list = 
    match list with
    | x :: xs -> 
        match filter (fun y -> p y x) xs with
        | [] -> sames p xs
        | ys -> (x :: ys) :: sames p ( filter (fun z -> z <> x) xs )
    | [] -> []

let elem p x list = foldl (fun acc y -> p y x || acc ) list false 

let leftOnly p l r = filter (fun x -> not (elem p x r)) l
