﻿using System.Linq;
using PowerCode;
using Xunit;

namespace GitCompletionTests
{
    public class GitCompletionTests
    {
        private static string[] GetGitLog(string command) =>
            new[]
            {
                "afcff36f adding resharper settings to gitignore",
                "2a9ace83 Adding tests and fixing issues",
                "8e2a3ad3 Changing completion tests to distinguish between tests where the expected results are just a subset and those where all results are expected.",
                "21175387 Enabling TypeInference use of runtime SafeExprEval for completion",
                "2f74cd0a Moving CurrentTypeDefinitionAst to TypeInferenceContext",
                "48e07bf6 Replace CompletionExecutionHelper with PowerShellExecutionHelper",
                "6f02a0aa Removing the Ast.GetInferredType virtual methods from all ASTs",
                "51253d25 Adding TypeInferenceVisitor with methods transformed from Ast.GetInferredType",
                "06020f34 Fix New-Item to create correct symlink type (#2915) (#3509)",
                "2851f7e8 Accept `-i` for an interactive shell (#3558)",
                "47ec6b2a Use IntPtr(-1) for INVALID_HANDLE_VALUE instead of IntPtr.Zero (#3544)",
                "4ad4b194 Convert tab indentations to spaces in *.cs files (#3551)",
                "acec58c0 Make the output of $PSVersionTable in alphabetical order (#3530)",
                "f0c0176a Typo in Format-Hex (#3539)",
                "35168721 Use /bin/bash, fixes #3525",
                "8d4db01a Add sudo for adding cert for OpenSUSE (#3524)",
                "68bcd4b5 Removed trailing whitespace (#3485)",
                "18c28f8f Fix markdown lint issues for SSH Remoting demo and enable related tests (#3484)",
                "4697dd2b Adding public ValidRootDrives property to ValidateDrive (#3510)",
                "753b1965 Fix crash at startup when env:HOME not set (#3437)"
            };

        private static string[] GetGitRemoteRefs(string command) =>
            new[]
            {
                "b80765d703f2e3766b4838f14455c40da255fa88 refs/remotes/origin/ForEach",
                "636a14cff5672bddc638786a42802681bc4d382d refs/remotes/origin/HEAD",
                "fea270412167ad3e4e5a5297642dcb7d5f9c51ee refs/remotes/origin/ext-method",
                "5f308d5ea80699ee0379c341ed5de070ed49944c refs/remotes/upstream/ForEach",
                "1c2c3a223f024371677846d2a8b5e4a6079e8cd7 refs/remotes/upstream/import-datafile",
                "38f6dad857ee8a70a1d7b23c67f2af32ffe50d40 refs/remotes/upstream/make-csproj-build"
            };

        private static string[] GetGitRemoteUrl(string command) =>
            new[]
            {
                "origin	https://github.com/powercode/PowerShell (fetch)",
                "origin	https://github.com/powercode/PowerShell (push)",
                "upstream	https://github.com/powershell/PowerShell (fetch)",
                "upstream	https://github.com/powershell/PowerShell (push)"
            };

        [Fact]
        public void CanCompleteGetGitFetchRemote()
        {
            using var scope = new GitExecuterScope(executer: Git.GitExecuter);
            Git.GitExecuter = GetGitRemoteRefs;
            var completions = "git fetch ".CompleteInput();
            Assert.Equal(2, actual: completions.Count);
            Assert.Equal("upstream", actual: completions[1].CompletionText);
        }

        [Fact]
        public void CanCompleteGetGitFetchRemoteRef()
        {
            using var scope = new GitExecuterScope(executer: Git.GitExecuter);
            Git.GitExecuter = GetGitRemoteRefs;
            var completions = "git fetch origin".CompleteInput();
            Assert.Equal(3, actual: completions.Count);
            Assert.Equal("ForEach", actual: completions[0].CompletionText);
        }


        [Fact]
        public void CanCompleteGitCommandOptions()
        {
            var res = "git add ".CompleteInput();
            Assert.Equal("--", actual: res[0].CompletionText);
            Assert.Equal("-A", actual: res[1].CompletionText);
            Assert.Equal("--all", actual: res[2].CompletionText);
        }

        [Fact]
        public void CanCompleteGitCommandPart()
        {
            var res = "git ad".CompleteInput("ad");
            Assert.Equal("add", actual: res[0].CompletionText);
        }

        [Fact]
        public void CanCompleteGitCommands()
        {
            var res = "git ".CompleteInput();
            Assert.Equal("add", actual: res[0].CompletionText);
            var diff = res.FirstOrDefault(c => c.CompletionText == "diff");
            Assert.NotNull(@object: diff);
        }

        [Fact]
        public void CanCompleteGitDiffOptions()
        {
            using var scope = new GitExecuterScope(executer: Git.GitExecuter);
            Git.GitExecuter = GetGitLog;
            var res = "git diff ".CompleteInput();
            Assert.Equal("afcff36f", actual: res[0].CompletionText);
            Assert.Equal("Moving CurrentTypeDefinitionAst to TypeInferenceContext", actual: res[4].ToolTip);
        }

        [Fact]
        public void CanGetGitRemoteRefs()
        {
            using var scope = new GitExecuterScope(executer: Git.GitExecuter);
            Git.GitExecuter = GetGitRemoteRefs;
            var remoteRefs = Git.RemoteRefs().ToArray();
            Assert.Equal("ForEach", actual: remoteRefs[0].Ref);
            Assert.Equal("fea270412167ad3e4e5a5297642dcb7d5f9c51ee", actual: remoteRefs[2].Commit);
            Assert.Equal("origin", actual: remoteRefs[1].Remote);
            Assert.Equal("origin/ext-method", actual: remoteRefs[2].RemoteRef);
        }

        [Fact]
        public void CanGetGitRemoteUrls()
        {
            using (new GitExecuterScope(executer: Git.GitExecuter))
            {
                Git.GitExecuter = GetGitRemoteUrl;
                var remoteRefs = Git.Remotes().ToArray();
                Assert.Equal("origin", actual: remoteRefs[0].Name);
                Assert.Equal("https://github.com/powershell/PowerShell (fetch)", actual: remoteRefs[1].FetchUrl);
                Assert.Equal("https://github.com/powershell/PowerShell (fetch)", actual: remoteRefs[1].FetchUrl);
            }
        }
    }
}