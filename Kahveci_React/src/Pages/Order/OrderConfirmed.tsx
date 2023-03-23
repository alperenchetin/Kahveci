import { useParams } from "react-router-dom";
import React from "react";

let confirmedImage = require("../../Assets/Images/coffee-bg.jpg");
function OrderConfirmed() {
  const { id } = useParams();
  return (
    <div className="w-100 text-center d-flex justify-content-center align-items-center">
      <div>
        <i
          style={{ fontSize: "7rem" }}
          className="bi bi-check2-circle text-success"
        ></i>
        <div className="pb-5">
          <h2 className=" text-success">Sipariş Onaylandı!</h2>
          <h5 className="mt-3">Sipariş Numarası: {id}</h5>
          <p>Siparişiniz hazırlanıyor</p>
          <img
            src={confirmedImage}
            style={{ width: "40%", borderRadius: "30px" }}
          ></img>
        </div>
      </div>
    </div>
  );
}

export default OrderConfirmed;
