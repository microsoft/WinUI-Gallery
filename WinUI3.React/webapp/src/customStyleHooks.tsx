import {
  FluentProviderCustomStyleHooks,
  makeStyles,
  shorthands,
  tokens,
  mergeClasses,
  useTextStyles_unstable,
  checkboxClassNames,
  textareaClassNames,
} from "@fluentui/react-components";
import * as React from "react";

import type {
  ButtonState,
  CheckboxState,
  LinkState,
  TextareaState,
} from "@fluentui/react-components";

import { Square16Regular, Checkmark16Regular } from "@fluentui/react-icons";

const useWinUI3Styles = makeStyles({
  buttonRoot: {
    fontWeight: "normal",
    minWidth: "120px",
    // TODO: adding round, gradient borders is tricky, but possible
    "&:hover": {
      backgroundColor: "rgba(0, 95, 184, 0.90)",
    },
    "&:hover:active": {
      backgroundColor: "rgba(0, 95, 184, 0.80)",
    },
  },
  checkboxRoot: {
    "&:after": {
      borderRadius: "5px !important",
    },
  },
  checkboxRootChecked: {
    [`&.${checkboxClassNames.indicator}`]: {
      backgroundColor: "#005FB8",
    },
    [`&:hover .${checkboxClassNames.indicator}`]: {
      backgroundColor: "#005FB8E5",
    },
    [`&:active .${checkboxClassNames.indicator}`]: {
      backgroundColor: "#005FB8CC",
      color: "#FFFFFFB2",
      border: "1px solid #00000037",
    },
  },
  checkboxRootNotChecked: {
    [`&:hover .${checkboxClassNames.indicator}`]: {
      backgroundColor: "#0000000F",
    },
    [`&:active .${checkboxClassNames.indicator}`]: {
      backgroundColor: "#00000018",
      border: "1px solid #00000037",
    },
  },

  checkboxIndicator: {
    borderRadius: "5px",
    height: "20px",
    width: "20px",
  },
  checkboxIndicatorNotChecked: {
    backgroundColor: "#00000006",
  },

  linkRoot: {
    background: "green",
    textDecorationLine: "underline",
    "&:hover": {
      textDecorationLine: "none",
    },
  },

  textareaRoot: {
    backgroundColor: "rgba(255, 255, 255, 0.70)",
    border: "1px solid #0000000F",
    "&:hover:not(:focus-within)": {
      backgroundColor: "#F9F9F980",
      [`& .${textareaClassNames.textarea}`]: {
        color: "#0000009B",
      },
    },
    "&:focus-within": {
      backgroundColor: "#ffffff",
    },

    [`&:focus-within .${textareaClassNames.textarea}`]: {
      color: "#0000009B",
    },
  },
  textareaInput: {
    minHeight: "112px",
  },
});

const useButtonStyles = (state: unknown) => {
  const winUI3Styles = useWinUI3Styles();
  const componentState = state as ButtonState;
  componentState.root.className = mergeClasses(
    componentState.root.className,
    winUI3Styles.buttonRoot
  );
};

const useCheckboxStyles = (state: unknown) => {
  const winUI3Styles = useWinUI3Styles();
  const componentState = state as CheckboxState;
  let icon;
  if (componentState.checked === "mixed") {
    icon = <Square16Regular />;
  } else if (componentState.checked) {
    icon = <Checkmark16Regular />;
  }
  if (componentState.indicator) {
    componentState.indicator.children = icon;
  }
  componentState.root.className = mergeClasses(
    componentState.root.className,
    winUI3Styles.checkboxRoot,
    componentState.checked
      ? winUI3Styles.checkboxRootChecked
      : winUI3Styles.checkboxRootNotChecked
  );
  if (componentState.indicator) {
    componentState.indicator.className = mergeClasses(
      componentState.indicator.className,
      winUI3Styles.checkboxIndicator,
      !componentState.checked && winUI3Styles.checkboxIndicatorNotChecked
    );
  }
};

const useLinkStyles = (state: unknown) => {
  const winUI3Styles = useWinUI3Styles();
  const componentState = state as LinkState;
  componentState.root.className = mergeClasses(
    componentState.root.className,
    winUI3Styles.linkRoot
  );
};

const useTextareaStyles = (state: unknown) => {
  const winUI3Styles = useWinUI3Styles();
  const componentState = state as TextareaState;
  componentState.root.className = mergeClasses(
    componentState.root.className,
    winUI3Styles.textareaRoot
  );
  componentState.textarea.className = mergeClasses(
    componentState.textarea.className,
    winUI3Styles.textareaInput
  );
};

export const customStyleHooks: FluentProviderCustomStyleHooks = {
  useButtonStyles_unstable: useButtonStyles,
  useCheckboxStyles_unstable: useCheckboxStyles,
  useLinkStyles_unstable: useLinkStyles,
  useTextareaStyles_unstable: useTextareaStyles,
};
