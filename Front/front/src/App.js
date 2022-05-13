import "./App.css";
import Home from "./components/Home/Home";
import {BrowserRouter as Router ,Routes, Route} from 'react-router-dom'
import Login from "./components/Login/Login";
import Register from "./components/Register/Register";

function App() {
  

  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path="/" element={<Login/>}/>
          <Route path="/accueil" element={<Home/>}/>
          <Route path="/register" element={<Register/>}/>
         </Routes>
      </Router>
    </div>
  );
}

export default App;
