import React, { useEffect, useState } from "react";
import axios from "axios";
import "./listeFilm.css";
import { useSelector } from "react-redux";
import * as AiIcons from "react-icons/ai";
import * as FcIcons from "react-icons/fc";

const ListeFilm = () => {
  const [dataMovie, setDataMovie] = useState([]);
  const user = useSelector((state) => state.user);

  useEffect(() => {
    axios
      .get(`${process.env.REACT_APP_API_MOVIE}/films/${user.id}`)
      .then((res) => setDataMovie(res.data.films))

      .catch((err) => console.error(err));
  }, [user.id]);

  const handleLike = (d, index, user_liked) => {
    axios
      .get(`${process.env.REACT_APP_API_MOVIE}/SETLIKE/${user.id}/${d}`)
      .then((res) => setDataMovie(res.data.films))
      .catch((err) => console.error(err));
  };

  const handleUnlike =(d)=> {
    axios
    .get(`${process.env.REACT_APP_API_MOVIE}/UNSETLIKE/${user.id}/${d}`)
    .then((res) => setDataMovie(res.data.films))
    .catch((err) => console.error(err));
  }

  return (
    <div className="listefilm">
      <ul className="listefilm_list">
        {dataMovie &&
          dataMovie.map((d, index) => (
            <li
              className="listefilm_list_item"
              key={index}
              style={{
                backgroundImage: `url(https://image.tmdb.org/t/p/w500${d.image_url})`,
              }}
            >
              <div className="listefilm_list_global_text">
                <p className="listefilm_list_title">{d.title}</p>
                <p className="listefilm_list_text">{d.synopsis}</p>
                {d.user_liked ? (
                  <FcIcons.FcLike
                    name="unlike"
                    onClick={() => handleUnlike(d.id, index)}
                    className="listefilm_iconLike"
                  />
                ) : (
                  <AiIcons.AiOutlineHeart
                    name="like"
                    onClick={() => handleLike(d.id)}
                    className="listefilm_iconNotlike"
                  />
                )}
              </div>
            </li>
          ))}
      </ul>
    </div>
  );
};
export default ListeFilm;
