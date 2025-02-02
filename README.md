﻿# TableCloth2 프로젝트 Quickstart 가이드

NOTE: 식탁보 vNext를 개발 중입니다. 이 리포지터리는 형상 관리를 위해서만 유지 관리되며, 추후 본 리포지터리에 히스토리 없이 병합될 예정입니다.

## 프로젝트 개요

TableCloth2는 Windows Sandbox를 활용하여 1회용 가상 머신을 생성하고, Spork는 해당 가상 머신 안에서 미리 지정된 소프트웨어들을 자동으로 설치하는 도구입니다. 이 둘은 함께 작동하여 안전하고 일관된 가상 실행 환경을 제공합니다.

## 사전 요구 사항

- TableCloth2 개발을 위해서는 Visual Studio 2022, .NET 9 SDK, Git 클라이언트가 필요합니다.
- 빌드와 테스트를 진행하려면, Windows Sandbox를 실행할 수 있는 Windows 11 Pro 이상의 OS와 하드웨어가 필요합니다.

## 프로젝트 구조

### TableCloth2.Shared

공통으로 사용되는 서비스와 유틸리티 클래스들이 포함되어 있습니다. 애플리케이션 전반에서 공유되는 모델과 계약(interfaces)을 정의합니다.

### TableCloth2.TableCloth

Windows Sandbox 환경을 구성하고 가상 머신을 생성하는 메인 애플리케이션 로직이 포함되어 있습니다. WindowsSandboxComposer를 통해 Sandbox 구성 파일을 생성합니다. 사용자 설정에 따라 가상 머신을 생성하고 실행합니다.

### TableCloth2.Spork

Windows Sandbox 내부에서 실행되며, 미리 지정된 소프트웨어들을 자동으로 설치합니다. SporkBootstrapper를 통해 필요한 초기화 작업을 수행합니다. 또한 설치 스크립트와 자동화된 설치 프로세스를 관리합니다.

## 빌드 및 실행

1. 저장소 복제

```powershell
git clone [저장소 URL]
```

2. Visual Studio에서 솔루션 열기

솔루션 파일(TableCloth2.sln)을 Visual Studio 2022로 엽니다.

3. NuGet 패키지 복원

솔루션을 빌드하기 전에 NuGet 패키지를 복원합니다.

4. 솔루션 빌드

`Ctrl + Shift + B`를 눌러 솔루션을 빌드합니다.

5. 프로젝트 실행

`TableCloth2.TableCloth` 프로젝트를 시작 프로젝트로 설정하고 F5를 눌러 디버깅 모드로 실행합니다.

## 기여 방법

1. 이슈 확인

작업하려는 기능이나 버그에 대한 이슈가 있는지 확인합니다.

2. 브랜치 생성

```powershell
git checkout -b feature/작업-내용
```

3. 코드 수정 및 커밋

코드를 수정하고 변경 사항을 커밋합니다.

```powershell
git commit -m "작업 내용 설명"
```

4. 브랜치 푸시

```powershell
git push origin feature/작업-내용
```

5. Pull Request 생성

GitHub에서 Pull Request를 생성하고 코드 리뷰를 요청합니다.

## 코드 스타일 가이드라인

- C# 13.0 기능 사용: 최신 C# 언어 기능을 적극 활용합니다.
- 네이밍 컨벤션: PascalCase를 사용하여 클래스 및 메서드 이름을 지정합니다. private 필드에는 _ 접두사를 사용합니다 (예: _sessionService).
- 비동기 프로그래밍: async/await 패턴을 따르며, 비동기 메서드 이름은 Async로 끝냅니다.
- 주석 및 문서화: 공용 메서드와 클래스에는 XML 주석을 작성하여 문서화합니다.

## 주요 클래스 및 서비스 설명

### TableCloth2.TableCloth

- WindowsSandboxComposer: Windows Sandbox 환경의 구성 요소를 XML 문서로 작성합니다. 가상 머신 실행을 위한 설정 파일(.wsb)을 생성합니다. 사용자 설정에 따라 공유 폴더, 가상 GPU 사용 여부, 오디오 및 비디오 입력 등을 구성합니다.
- TableClothBootstrapper: 애플리케이션의 초기화 작업을 수행합니다. 세션 관리를 통해 중복 실행을 방지합니다.

### TableCloth2.Spork

- SporkBootstrapper: Windows Sandbox 내부에서 실행되며, 필요한 초기 설정을 수행합니다. 자동 설치 스크립트를 실행하여 소프트웨어를 설치합니다.

### 공유 서비스

- ProcessUtility: 현재 프로세스의 정보 및 애플리케이션 재시작 기능을 제공합니다. 관리자 권한 확인 및 프로세스 파일 경로 가져오기 등의 기능 포함.
- SessionService: 애플리케이션 세션 관리를 담당하며, 단일 실행 인스턴스를 보장합니다.
- KnownPathsService: 애플리케이션에서 사용하는 주요 디렉터리 경로를 관리합니다.

## 실행 흐름 이해

1. TableCloth 실행: 사용자가 TableCloth를 실행하여 원하는 설정을 선택합니다. Windows Sandbox 구성 파일을 생성하고 가상 머신을 시작합니다.
2. Windows Sandbox 내에서 Spork 실행: 가상 머신이 시작되면, 자동으로 Spork가 실행됩니다. Spork는 미리 지정된 소프트웨어들을 자동으로 설치합니다.
3. 사용자 작업: 필요한 작업을 가상 머신 안에서 수행합니다. 작업 완료 후 가상 머신을 종료하면 모든 데이터가 초기화됩니다.

## 테스트 및 디버깅

- Windows Sandbox 환경 테스트: Windows 10 Pro 이상에서 Windows Sandbox 기능을 활성화해야 합니다. 가상 머신 내부의 동작을 테스트하려면 Spork 프로젝트를 별도로 실행하거나 디버깅합니다.
- 로깅 및 오류 처리: ILogger 인터페이스를 활용하여 애플리케이션의 동작을 로깅합니다. 예외 처리를 철저히 하여 예상치 못한 종료를 방지합니다.

## 문의 및 지원

- 이슈 트래커: 버그 신고나 기능 요청은 GitHub의 이슈 트래커를 통해 제출해주세요.
- 프로젝트 관리자: 추가 지원이 필요하면 프로젝트 관리자나 팀에 문의하시기 바랍니다.

함께 TableCloth2 프로젝트를 발전시켜 나가길 기대합니다. 기여해 주셔서 감사합니다!
