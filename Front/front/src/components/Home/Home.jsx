import React from 'react'
import ListeFilm from '../ListeFilm/ListeFilm'
import Navbar from '../Navbar/Navbar'
import "./home.css"

const Home = () => {
  return (
    <div className='Home'>
        <div className="home_navbar">
            <Navbar/>
        </div>
        <div className="home_content">
            <ListeFilm/>
        </div>
    </div>
  )
}

export default Home