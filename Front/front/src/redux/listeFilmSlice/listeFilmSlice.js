import { createSlice } from "@reduxjs/toolkit";

export const listeFilmSlice = createSlice({
    name:"listefilm",
    initialState:[],
    reducers:{
        getFilm: (state,action)=> {
            return {state,...action.payload}
        }
    }
})

export const {getFilm}= listeFilmSlice.actions