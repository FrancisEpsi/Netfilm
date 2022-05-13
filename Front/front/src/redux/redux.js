import { configureStore } from '@reduxjs/toolkit'
import { applyMiddleware, combineReducers } from 'redux'
import persistReducer from 'redux-persist/es/persistReducer'
import persistStore from 'redux-persist/es/persistStore'
import thunk from 'redux-thunk'
import storage from 'redux-persist/lib/storage' 
import { composeWithDevTools } from '@redux-devtools/extension'
import { userSLice } from './userSlice/userSlice'
import { listeFilmSlice } from './listeFilmSlice/listeFilmSlice'


const persistConfig ={
    key:"root",
    version:1,
    storage
}


const reducer = combineReducers({
    user: userSLice.reducer,
    listefilm : listeFilmSlice.reducer,
    persistConfig
})

const persistedReducer = persistReducer(persistConfig,reducer)

export const store =configureStore({
    reducer:persistedReducer,
    devTools : composeWithDevTools(applyMiddleware(thunk)),
    middleware:[thunk]
})
 export let persistor = persistStore(store)