# ChiecNonKiDieu

Client:
	- Tạo phòng:				101 <length> <mã phòng>
	- Tham gia phòng: 			103 <length> <mã phòng>
	- Tham gia chơi: 			104 0 0
	- Quay vòng quay:			110 <length> <mã ô quay>
	- Click từ khoá:			111 <length> <mã từ khoá>
	- Click từ gợi ý:			112 <length> <mã từ gợi ý>
	- Thoát:           			121 0 0
Server:
	- Tạo phòng thành công:		201 <length> <mã phòng>
	- Tạo phòng thất bại:		202 <length> <mã phòng>
	- Gửi danh sách người chơi: 203 <length> <player1 player2 player3>
	- Bắt đầu trờ chơi: 		210 0 0
	- Gửi câu hỏi :				220 <length> <câu hỏi>
	- Gửi mở từ khoá:			221 <length> <mã từ khoá>
	- Gửi mở từ gợi ý:			222 <length> <mã từ gợi ý>
	- Gửi luợt chơi: 			223 <length> <số thư tự người chơi trong phòng>
	- Thao tác không hợp lệ: 	250 0 0 
