FROM gcc:13-bookworm

# Tạo user không có quyền root để chạy code thí sinh an toàn
RUN useradd -m -s /bin/bash runner
USER runner
WORKDIR /sandbox

