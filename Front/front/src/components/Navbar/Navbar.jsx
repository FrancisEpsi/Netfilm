import React from 'react';
import { useSelector } from 'react-redux';
import './navbar.css'




const Navbar =()=>{

    const user = useSelector(state=> state.user)

    return(

        <div className='navBar'>
            <nav className= "navBar-nav">
            <ul className='navBar-list'>
                <li>
                    <span className='navBar-list'>Bonjour {user.last_name}</span>
                </li>
                <li>
                    <a  className="navBar-list" href="/#project"><span>Deconnection</span></a>
                </li>
                
            </ul>
            </nav>
        </div>
       )
}


export default Navbar