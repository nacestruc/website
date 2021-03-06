
[<RequireQualifiedAccess>]
module MunicipalitiesChart

open Elmish

open Feliz
open Feliz.ElmishComponents

open Types

type State =
    { Data : RegionsData
      Regions : Region list
      ShowAll : bool }

type Msg =
    | ToggleShowAll

let regionTotal (region : Region) : int =
    region.Municipalities
    |> List.map (fun city -> city.PositiveTests)
    |> List.choose id
    |> List.sum

let init (data : RegionsData) : State * Cmd<Msg> =
    let lastDataPoint = List.last data
    let regions =
        lastDataPoint.Regions
        |> List.sortByDescending (fun region -> regionTotal region)

    { Data = data ; Regions = regions ; ShowAll = false }, Cmd.none

let update (msg: Msg) (state: State) : State * Cmd<Msg> =
    match msg with
    | ToggleShowAll ->
        { state with ShowAll = not state.ShowAll }, Cmd.none

let excludeMunicipalities = Set.ofList ["kraj"]

let calculateDoublingTime (v1 : {| Day : int ; PositiveTests : int |}) (v2 : {| Day : int ; PositiveTests : int |}) =
    let v1, v2, dt = float v1.PositiveTests, float v2.PositiveTests, float (v2.Day - v1.Day)
    if v1 = v2 then None
    else log10 2.0 / log10 ((v2 / v1) ** (1.0 / dt)) |> Some

let renderMunicipalities (state : State) dispatch =
    let barMaxHeight = 50
    let showMaxBars = 10
    let collapsedMnicipalityCount = 24
    let doublingTimeInterval = 4

    let pivotedData = seq {
        for dataPoint in state.Data do
            for region in dataPoint.Regions do
                for municipality in region.Municipalities do
                    if not (Set.contains municipality.Name excludeMunicipalities)  then
                        yield {| Date = dataPoint.Date
                                 Region = region.Name
                                 Municipality = municipality.Name
                                 PositiveTests = municipality.PositiveTests |} }

    let sortedData =
        pivotedData
        |> Seq.groupBy (fun d -> {| Region = d.Region ; Municipality = d.Municipality |})
        |> Seq.sortWith (fun (_, data1) (_, data2) ->
            let last1, last2 = (Seq.last data1), (Seq.last data2)
            match last1.PositiveTests, last2.PositiveTests with
            | None, None -> System.String.Compare(last1.Municipality, last2.Municipality)
            | Some v, None -> -1
            | None, Some v -> 1
            | Some v1, Some v2 ->
                if v1 > v2 then -1
                else if v1 < v2 then 1
                else System.String.Compare(last1.Municipality, last2.Municipality))

    let trimmedData =
        if state.ShowAll = true
        then sortedData
        else Seq.take collapsedMnicipalityCount sortedData

    trimmedData
    |> Seq.map (fun (key, data) ->
        let trimmedData = Seq.skip ((Seq.length data) - showMaxBars) data

        let maxValue =
            try
                trimmedData
                |> Seq.map (fun d -> d.PositiveTests)
                |> Seq.filter Option.isSome
                |> Seq.max
            with
                | _ -> None

        let reversedDoublingTimeValues =
            trimmedData
            |> Seq.mapi (fun i p -> i, p.PositiveTests)
            |> Seq.choose (fun (i, p) ->
                match p with
                | None -> None
                | Some v -> Some {| Day = i ; PositiveTests = v |})
            |> Seq.rev
            |> Array.ofSeq

        let doublingTime =
            match reversedDoublingTimeValues.Length with
            | 0 | 1 -> None
            | length ->
                if length >= doublingTimeInterval then
                    calculateDoublingTime reversedDoublingTimeValues.[doublingTimeInterval - 1] reversedDoublingTimeValues.[0]
                else
                    calculateDoublingTime reversedDoublingTimeValues.[length - 1] reversedDoublingTimeValues.[0]

        printfn "%s %A" key.Municipality doublingTime

        let bars =
            match maxValue with
            | None -> Seq.empty
            | Some maxValue ->
                seq {
                    for i, d in trimmedData |> Seq.mapi (fun i d -> i, d) do
                        match d.PositiveTests with
                        | None ->
                            yield Html.div [
                                prop.className "bar bar--empty"
                            ]
                        | Some positiveTests ->
                            yield Html.div [
                                prop.className "bar-wrapper"
                                prop.children [
                                    Html.div [
                                        prop.className "bar"
                                        prop.style [ style.height (positiveTests * barMaxHeight / maxValue) ] ]
                                    Html.div [
                                        prop.className "total-and-date total-and-date--hover"
                                        prop.children [
                                            Html.div [
                                                prop.className "total"
                                                prop.text positiveTests ]
                                            Html.div [
                                                prop.className "date"
                                                prop.text (sprintf "%d. %s" d.Date.Day (Utils.monthNameOfdate d.Date)) ]
                                        ]
                                    ]
                                ]
                            ]
                    }

        let lastDataPoint = Seq.last data

        let totalPositiveTests =
            match lastDataPoint.PositiveTests with
            | None -> ""
            | Some v -> v.ToString()

        Html.div [
            prop.className "municipality"
            prop.children [
                Html.div [
                    prop.className "name"
                    prop.text (
                        match Utils.Dictionaries.municipalities.TryFind key.Municipality with
                        | None -> key.Municipality
                        | Some municipality -> municipality.Name)
                ]
                Html.div [
                    prop.className "positive-tests"
                    prop.children [
                        Html.div [
                            prop.className "bars"
                            prop.children bars
                        ]
                        Html.div [
                            prop.className "total-and-date"
                            prop.children [
                                Html.div [
                                    prop.className "total"
                                    prop.text totalPositiveTests ]
                                Html.div [
                                    prop.className "date"
                                    prop.text (sprintf "%d. %s" lastDataPoint.Date.Day (Utils.monthNameOfdate lastDataPoint.Date)) ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    )

let renderShowMore showAll dispatch =
    Html.div [
        prop.className "show-all"
        prop.children [
            Html.div [
                Html.button [
                    prop.className "btn btn-primary btn-sm"
                    prop.text (if showAll then "Prikaži manj občin" else "Prikaži več občin")
                    prop.onClick (fun _ -> dispatch ToggleShowAll)
                ]
            ]
        ]
    ]

let render (state : State) dispatch =
    Html.div [
        prop.children [
            Html.div [
                prop.className "municipalities"
                prop.children (renderMunicipalities state dispatch) ]
            renderShowMore state.ShowAll dispatch
        ]
    ]

type Props = {
    data : RegionsData
}

let municipalitiesChart (props : Props) =
    React.elmishComponent("MunicipalitiesChart", init props.data, update, render)
