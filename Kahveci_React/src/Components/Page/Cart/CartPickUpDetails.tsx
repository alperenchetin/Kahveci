import React, { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useInitiatePaymentMutation } from "../../../Apis/paymentApi";
import { inputHelper } from "../../../Helper";
import { apiResponse, cartItemModel } from "../../../Interfaces";
import { RootState } from "../../../Storage/Redux/store";
import { MiniLoader } from "../Common";
import { useNavigate } from "react-router";

export default function CartPickUpDetails() {
  const [loading, setLoading] = useState(false);
  const shoppingCartFromStore: cartItemModel[] = useSelector(
    (state: RootState) => state.shoppingCartStore.cartItems ?? []
  );
  const userData = useSelector((state: RootState) => state.userAuthStore);

  let grandTotal = 0;
  let totalItems = 0;
  const initialUserData = {
    name: userData.fullName,
    email: userData.email,
    phoneNumber: "",
  };
  shoppingCartFromStore?.map((cartItem: cartItemModel) => {
    totalItems += cartItem.quantity ?? 0;
    grandTotal += (cartItem.menuItem?.price ?? 0) * (cartItem.quantity ?? 0);
    return null;
  });
  const navigate = useNavigate();
  const [userInput, setUserInput] = useState(initialUserData);
  const [initiatePayment] = useInitiatePaymentMutation();
  const handleUserInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    const tempData = inputHelper(e, userInput);
    setUserInput(tempData);
  };

  useEffect(() => {
    setUserInput({
      name: userData.fullName,
      email: userData.email,
      phoneNumber: "",
    });
  }, [userData]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setLoading(true);

    const { data }: apiResponse = await initiatePayment(userData.id);

    navigate("/payment", {
      state: { apiResult: data?.result, userInput },
    });
  };

  return (
    <div className="border pb-5 pt-3">
      <h1 style={{ fontWeight: "300" }} className="text-center text-success">
        Alıcı Bilgileri
      </h1>
      <hr />
      <form onSubmit={handleSubmit} className="col-10 mx-auto">
        <div className="form-group mt-3">
          Alıcı İsmi
          <input
            type="text"
            value={userInput.name}
            className="form-control"
            placeholder="name..."
            name="name"
            onChange={handleUserInput}
            required
          />
        </div>
        <div className="form-group mt-3">
          Alıcı E-mail'i
          <input
            type="email"
            value={userInput.email}
            className="form-control"
            placeholder="email..."
            name="email"
            onChange={handleUserInput}
            required
          />
        </div>

        <div className="form-group mt-3">
          Telefon Numarası
          <input
            type="number"
            value={userInput.phoneNumber}
            className="form-control"
            placeholder="telefon numarası..."
            name="phoneNumber"
            onChange={handleUserInput}
            required
          />
        </div>
        <div className="form-group mt-3">
          <div className="card p-3" style={{ background: "ghostwhite" }}> 
            <h5>Ürün Sayısı : {totalItems}</h5>
            <h5>Toplam: {grandTotal.toFixed(2)} &nbsp; TL</h5>
          </div>
        </div>
        <button
          type="submit"
          className="btn btn-lg btn-success form-control mt-3"
          disabled={loading || shoppingCartFromStore.length == 0}
        >
          {loading ? <MiniLoader /> : "Siparişi Tamamla"}
        </button>
      </form>
    </div>
  );
}
