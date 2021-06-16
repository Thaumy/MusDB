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

let rec elem it list =
    match list with
    | x :: xs -> if it = x then true else elem it xs
    | [] -> false

let leftOnly l r = filter (fun x -> not (elem x r)) l
