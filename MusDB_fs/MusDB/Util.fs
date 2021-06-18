module Util


let rec map f list =
    match list with
    | x :: xs -> (f x) :: map f xs
    | [] -> []

let rec filter p list =
    match list with
    | x :: xs ->
        if p x then
            x :: filter p xs
        else
            filter p xs
    | [] -> []

let rec foldl f list acc =
    match list with
    | x :: xs -> foldl f xs (f acc x) 
    | [] -> acc

let rec foldr f list acc =
    match list with
    | x :: xs -> f x (foldr f xs acc)
    | [] -> acc

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
