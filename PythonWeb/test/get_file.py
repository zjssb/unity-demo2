import json
import os
import zipfile

import requests

localhost = 'http://127.0.0.1:5000/'

data = {
    'name': 'abc',
    'info': '123,456,789,test.txt'
}

if __name__ == '__main__':

    file = requests.get(localhost)
    print(file.text)

