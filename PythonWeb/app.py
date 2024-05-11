import os
import time
import zipfile
from flask import Flask, send_file, json, request

app = Flask(__name__)

data_path = './static/download'


def getFileSize(files):
    size = 0
    for file in files:
        size += os.path.getsize(f'{data_path}/{file}')
    return size


@app.route('/', methods=['POST'])
def hello_world():  # put application's code here
    names = request.form.get('names')
    name_list = names.split(',')

    # 将多余文件 和 缺少文件 名称填入列表中返回

    downloadFiles = os.listdir(data_path)
    deleteFiles = []
    for file in name_list:
        if file in downloadFiles:
            downloadFiles.remove(file)
        else:
            deleteFiles.append(file)

    size = getFileSize(downloadFiles)
    disp = {"download": downloadFiles, "delete": deleteFiles, 'size': size}
    out = json.dumps(disp)
    return out


# @app.route('/test', methods=['GET'])
# def update_test():
#     return send_file(app.static_folder + '\\download\\test.txt', as_attachment=True)


# @app.route('/download/<name>', methods=['GET'])
# def download(name):
#     filepath = app.static_folder + '\\download\\' + name
#     return send_file(filepath, as_attachment=True)

"""
    将列表与数据对比， 将不存在与列表中的数据打包
    将 数据包 返回
"""


@app.route('/update', methods=['POST'])
def update():
    # 一个列表，以 , 分隔元素
    fileName = request.form.get('name') + '_' + time.strftime('%y-%m-%d', time.localtime())


    info = request.form.get('files')
    infos = info.split(',')
    datas = os.listdir(data_path)

    for i in infos:
        if i not in datas:
            infos.remove(i)

    zip_obj = zipfile.ZipFile(f'./return/{fileName}.zip', 'w')
    for i in infos:
        zip_obj.write(f'{data_path}/{i}', arcname=f'{i}')
    zip_obj.close()

    return send_file(f'./return/{fileName}.zip', as_attachment=True)


if __name__ == '__main__':
    app.debug = True

    app.run()
