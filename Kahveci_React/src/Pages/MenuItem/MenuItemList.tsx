import React from "react";
import {
  useDeleteMenuItemMutation,
  useGetMenuItemsQuery,
} from "../../Apis/menuItemApi";
import { toast } from "react-toastify";
import { MainLoader } from "../../Components/Page/Common";
import { menuItemModel } from "../../Interfaces";
import { useNavigate } from "react-router";
function MenuItemList() {
  const [deleteMenuItem] = useDeleteMenuItemMutation();
  const { data, isLoading } = useGetMenuItemsQuery(null);
  const navigate = useNavigate();

  const handleMenuItemDelete = async (id: number) => {
    toast.promise(
      deleteMenuItem(id),
      {
        pending: "İsteğiniz gerçekleştiriliyor...",
        success: "Ürün Kaldırıldı",
        error: "Bir sorun meydana geldi",
      },
      {
        theme: "dark",
      }
    );
  };

  return (
    <>
      {isLoading && <MainLoader />}
      {!isLoading && (
        <div className="table p-5">
          <div className="d-flex align-items-center justify-content-between">
            <h1 className="text-info">Ürün Listesi</h1>

            <button
              className="btn btn-danger"
              onClick={() => navigate("/menuitem/menuitemupsert")}
            >
              Yeni Ürün Ekle
            </button>
          </div>

          <div className="p-2">
            <div className="row border">
              <div className="col-1">Fotoğraf</div>
              <div className="col-1">ID</div>
              <div className="col-2">Ürün Adı</div>
              <div className="col-2">Kategori</div>
              <div className="col-1">Fiyatı</div>
              <div className="col-2">Etiket</div>
              <div className="col-1">Düzenle &nbsp;/&nbsp;Sil</div>
            </div>

            {data.result.map((menuItem: menuItemModel) => {
              return (
                <div className="row border" key={menuItem.id}>
                  <div className="col-1">
                    <img
                      src={menuItem.image}
                      alt="Fotoğraf Yok"
                      style={{ width: "100%", maxWidth: "120px" }}
                    />
                  </div>
                  <div className="col-1">{menuItem.id}</div>
                  <div className="col-2">{menuItem.name}</div>
                  <div className="col-2">{menuItem.category}</div>
                  <div className="col-1">{menuItem.price} TL</div>
                  <div className="col-2">{menuItem.specialTag}</div>
                  <div className="col-1">
                    <button className="btn btn-success">
                      <i
                        className="bi bi-pen"
                        onClick={() =>
                          navigate("/menuitem/menuitemupsert/" + menuItem.id)
                        }
                      ></i>
                    </button>
                    <button
                      className="btn btn-danger mx-2"
                      onClick={() => handleMenuItemDelete(menuItem.id)}
                    >
                      <i className="bi bi-trash3"></i>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </>
  );
}

export default MenuItemList;
