//using UnityEngine;

//public class XoayCamera : MonoBehaviour
//{
//    public Transform player; // Gán Player vào đây
//    public float khoangCach = 5f; // Khoảng cách từ Camera đến Player
//    public float doCao = 2f; // Độ cao của Camera so với Player
//    public float gocNghieng = 90f; // Góc nghiêng cố định của Camera

//    void LateUpdate()
//    {
//        if (player == null) return;

//        // Tính toán góc xoay chỉ dựa trên trục Y của Player và góc nghiêng cố định
//        Quaternion xoay = Quaternion.Euler(gocNghieng, player.eulerAngles.y, 0);

//        // Tính vị trí mới cho Camera dựa trên góc xoay và khoảng cách
//        Vector3 viTriMoi = player.position - xoay * Vector3.forward * khoangCach;
//        viTriMoi.y = player.position.y + doCao;

//        // Cập nhật vị trí và góc xoay của Camera
//        transform.position = viTriMoi;
//        transform.rotation = xoay;
//    }
//}